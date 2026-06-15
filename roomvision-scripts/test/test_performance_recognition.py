"""
Performance benchmark for the face recognition pipeline.

Dataset layout expected on disk:
    <dataset_dir>/
        alice/
            foto1.jpg
            foto2.jpg
            ...
        bob/
            foto1.jpg
            ...
        unknown/          ← optional: people NOT in the DB (ground truth = "Unknown")
            foto1.jpg
            ...

The first image of every named folder is used to build the in-memory vector
store (embed phase).  All remaining images are used as test inputs (predict
phase).  Every image inside `unknown/` is tested but never embedded.

Run with a real dataset directory:
    DATASET_DIR=/path/to/dataset pytest test/test_performance_recognition.py -v -s

Without DATASET_DIR the test is skipped automatically.
"""

import os
import sys
import time
import uuid
from pathlib import Path
from typing import List, Tuple

import pytest

# ── conftest already patched heavy deps, but we need the real ones here ──────
# Remove stubs for the libraries we actually need real behaviour from.
for _mod in list(sys.modules):
    if _mod.startswith(("face_recognition", "cv2", "numpy", "mtcnn")):
        del sys.modules[_mod]

import cv2  # noqa: E402
import numpy as np  # noqa: E402
import face_recognition  # noqa: E402

from data_access.face import Face  # noqa: E402
from data_access.interface import VectorStore  # noqa: E402
from utilities.face_recognition_utils import (  # noqa: E402
    HogRecognitionUtil,
    CNNRecognitionUtil,
    MTCNNRecognitionUtil,
)

# ── scikit-learn ──────────────────────────────────────────────────────────────
try:
    from sklearn.metrics import (
        classification_report,
        accuracy_score,
        precision_score,
        recall_score,
        f1_score,
    )
except ImportError:  # pragma: no cover
    pytest.skip(
        "scikit-learn not installed — run: pip install scikit-learn",
        allow_module_level=True,
    )

# ── constants ─────────────────────────────────────────────────────────────────
DATASET_DIR = os.environ.get("DATASET_DIR", "")
TOLERANCES = [0.4, 0.5, 0.6]
UNKNOWN_LABEL = "Unknown"
DETECTORS = {
    "HOG":   HogRecognitionUtil,
    "CNN":   CNNRecognitionUtil,
    "MTCNN": MTCNNRecognitionUtil,
}

# ── in-memory vector store ────────────────────────────────────────────────────

class InMemoryVectorStore(VectorStore):
    """Minimal vector store backed by a plain list — no Qdrant required."""

    def __init__(self):
        self._records: List[Face] = []

    def create_payload(self, person_name: str) -> dict:
        return {"name": person_name}

    def add_vector(self, point_id, vector, payload=None):
        name = payload.get("name", UNKNOWN_LABEL) if payload else UNKNOWN_LABEL
        self._records.append(Face(id=str(point_id), person_name=name, embedding=list(vector)))

    def search(self, vector: list, limit: int = 1) -> Face | None:
        if not self._records:
            return None

        query = np.array(vector)
        best_face = min(
            self._records,
            key=lambda f: float(np.linalg.norm(np.array(f.getEmbedding()) - query)),
        )
        return best_face

# ── helpers ───────────────────────────────────────────────────────────────────

def _load_image(path: Path) -> np.ndarray | None:
    img_bgr = cv2.imread(str(path))
    if img_bgr is None:
        return None
    return cv2.cvtColor(img_bgr, cv2.COLOR_BGR2RGB)


def _embed(image: np.ndarray) -> list | None:
    encodings = face_recognition.face_encodings(image)
    return list(encodings[0]) if encodings else None


def _collect_images(dataset_dir: Path) -> Tuple[dict, dict]:
    """Return (train_set, test_set) where each is {person_name: [Path, ...]}."""
    train: dict = {}
    test: dict = {}

    for person_dir in sorted(dataset_dir.iterdir()):
        if not person_dir.is_dir():
            continue

        name = person_dir.name
        images = sorted(
            p for p in person_dir.iterdir()
            if p.suffix.lower() in {".jpg", ".jpeg", ".png"}
        )

        if not images:
            continue

        if name == UNKNOWN_LABEL.lower() or name == "unknown":
            test[UNKNOWN_LABEL] = images
        else:
            train[name] = [images[0]]
            if len(images) > 1:
                test[name] = images[1:]

    return train, test


def _build_store(train: dict) -> InMemoryVectorStore:
    store = InMemoryVectorStore()
    for person_name, paths in train.items():
        for path in paths:
            image = _load_image(path)
            if image is None:
                continue
            embedding = _embed(image)
            if embedding is None:
                print(f"  [WARN] no face found in train image: {path.name} ({person_name})")
                continue
            store.add_vector(
                point_id=str(uuid.uuid4()),
                vector=embedding,
                payload={"name": person_name},
            )
    return store


def _run_predictions(
    test: dict,
    store: InMemoryVectorStore,
    tolerance: float,
    detector_cls,
) -> Tuple[List[str], List[str]]:
    util = detector_cls(vector_store=store)
    y_true, y_pred = [], []
    detection_times, recognition_times = [], []

    for person_name, paths in test.items():
        for path in paths:
            image = _load_image(path)
            if image is None:
                continue

            t0 = time.perf_counter()
            face_locations = util.detect_faces(image)
            detection_times.append(time.perf_counter() - t0)

            if not face_locations:
                y_true.append(person_name)
                y_pred.append(UNKNOWN_LABEL)
                continue

            t1 = time.perf_counter()
            predictions = util.find_faces(
                image=image,
                unknown_faces_location=face_locations,
                vector_store=store,
                tolerance=tolerance,
            )
            recognition_times.append(time.perf_counter() - t1)

            predicted = predictions[0] if predictions else UNKNOWN_LABEL
            y_true.append(person_name)
            y_pred.append(predicted)

    total_images = len(y_true)
    avg_detection    = sum(detection_times) / len(detection_times) if detection_times else 0
    avg_recognition  = sum(recognition_times) / len(recognition_times) if recognition_times else 0
    avg_total        = avg_detection + avg_recognition

    timing = {
        "total_images":    total_images,
        "avg_detection_s": avg_detection,
        "avg_recognition_s": avg_recognition,
        "avg_total_s":     avg_total,
        "fps":             1 / avg_total if avg_total > 0 else 0,
    }

    return y_true, y_pred, timing


def _print_report(detector_name: str, tolerance: float, y_true: List[str], y_pred: List[str], timing: dict) -> dict:
    labels = sorted(set(y_true) | set(y_pred))

    accuracy  = accuracy_score(y_true, y_pred)
    precision = precision_score(y_true, y_pred, average="weighted", zero_division=0, labels=labels)
    recall    = recall_score(y_true, y_pred, average="weighted", zero_division=0, labels=labels)
    f1        = f1_score(y_true, y_pred, average="weighted", zero_division=0, labels=labels)

    separator = "=" * 60
    print(f"\n{separator}")
    print(f"  Detector        : {detector_name}")
    print(f"  Tolerance       : {tolerance}")
    print(f"  Images tested   : {timing['total_images']}")
    print(f"  Accuracy        : {accuracy:.3f}")
    print(f"  Precision       : {precision:.3f}")
    print(f"  Recall          : {recall:.3f}")
    print(f"  F1 Score        : {f1:.3f}")
    print(f"  Avg detection   : {timing['avg_detection_s']*1000:.1f} ms/img")
    print(f"  Avg recognition : {timing['avg_recognition_s']*1000:.1f} ms/img")
    print(f"  Avg total       : {timing['avg_total_s']*1000:.1f} ms/img  ({timing['fps']:.1f} FPS)")
    print(separator)
    print(classification_report(y_true, y_pred, zero_division=0))

    return {
        "detector": detector_name, "tolerance": tolerance,
        "accuracy": accuracy, "precision": precision, "recall": recall, "f1": f1,
        **timing,
    }


# ── test ──────────────────────────────────────────────────────────────────────

@pytest.mark.skipif(
    not DATASET_DIR,
    reason="Set DATASET_DIR=/path/to/dataset to run performance tests",
)
class TestFaceRecognitionPerformance:

    @pytest.fixture(scope="class")
    def dataset(self):
        path = Path(DATASET_DIR)
        assert path.exists(), f"DATASET_DIR does not exist: {path}"
        train, test = _collect_images(path)
        assert train, "No training images found — check dataset structure"
        assert test, "No test images found — each person folder needs at least 2 images"
        return train, test

    def test_performance_per_detector_and_tolerance(self, dataset):
        train, test = dataset
        store = _build_store(train)

        all_results = []
        for detector_name, detector_cls in DETECTORS.items():
            print(f"\n{'#' * 60}")
            print(f"  DETECTOR: {detector_name}")
            print(f"{'#' * 60}")
            for tolerance in TOLERANCES:
                y_true, y_pred, timing = _run_predictions(test, store, tolerance, detector_cls)
                assert len(y_true) > 0, f"No predictions were made for {detector_name}"
                result = _print_report(detector_name, tolerance, y_true, y_pred, timing)
                all_results.append(result)

        best_f1      = max(all_results, key=lambda r: r["f1"])
        fastest      = min(all_results, key=lambda r: r["avg_total_s"])

        print("\n" + "#" * 60)
        print("  SUMMARY")
        print("#" * 60)
        print(
            f"  Best F1    : Detector={best_f1['detector']}  "
            f"Tolerance={best_f1['tolerance']}  F1={best_f1['f1']:.3f}"
        )
        print(
            f"  Fastest    : Detector={fastest['detector']}  "
            f"Tolerance={fastest['tolerance']}  {fastest['avg_total_s']*1000:.1f} ms/img  ({fastest['fps']:.1f} FPS)"
        )

        assert best["f1"] > 0.0, "F1 score is 0 — something is wrong with the pipeline"
