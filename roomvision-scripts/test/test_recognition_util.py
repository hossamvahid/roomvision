"""Unit tests for RecognitionUtil and its subclasses."""
import sys
from unittest.mock import MagicMock, patch, call
import pytest

# conftest.py patches heavy deps before imports
import face_recognition as fr_mock          # already a MagicMock via conftest
from utilities.face_recognition_utils import (
    RecognitionUtil,
    MTCNNRecognitionUtil,
    HogRecognitionUtil,
    CNNRecognitionUtil,
)
from data_access.face import Face


# ── concrete stub so we can instantiate the abstract class ───────────────────

class _ConcreteUtil(RecognitionUtil):
    def detect_faces(self, image):
        return []


def _make_util(vector_store=None):
    vs = vector_store or MagicMock()
    return _ConcreteUtil(vector_store=vs), vs


# ── embed_face ────────────────────────────────────────────────────────────────

class TestEmbedFace:

    def test_returns_first_encoding_when_face_present(self):
        util, _ = _make_util()
        fake_encoding = [0.1, 0.2, 0.3]
        fr_mock.face_encodings.return_value = [fake_encoding]

        result = util.embed_face(image=MagicMock())

        assert result == fake_encoding

    def test_returns_none_when_no_encoding_found(self):
        util, _ = _make_util()
        fr_mock.face_encodings.return_value = []

        result = util.embed_face(image=MagicMock())

        assert result is None

    def test_passes_known_face_locations_when_provided(self):
        util, _ = _make_util()
        locations = [(10, 20, 30, 40)]
        fr_mock.face_encodings.return_value = [[0.5]]

        util.embed_face(image=MagicMock(), known_face_locations=locations)

        call_kwargs = fr_mock.face_encodings.call_args.kwargs
        assert call_kwargs["known_face_locations"] == locations

    def test_uses_medium_model(self):
        util, _ = _make_util()
        fr_mock.face_encodings.return_value = [[0.1]]

        util.embed_face(image=MagicMock())

        call_kwargs = fr_mock.face_encodings.call_args.kwargs
        assert call_kwargs.get("model") == "medium"


# ── compare_faces ─────────────────────────────────────────────────────────────

class TestCompareFaces:

    def test_returns_true_when_faces_match(self):
        util, _ = _make_util()
        fr_mock.compare_faces.return_value = [True]

        result = util.compare_faces([0.1], [0.2])

        assert result is True

    def test_returns_false_when_faces_do_not_match(self):
        util, _ = _make_util()
        fr_mock.compare_faces.return_value = [False]

        result = util.compare_faces([0.1], [0.9])

        assert result is False

    def test_passes_tolerance_to_library(self):
        util, _ = _make_util()
        fr_mock.compare_faces.return_value = [True]

        util.compare_faces([0.1], [0.2], tolerance=0.4)

        # assert_called_with checks the most recent call (mock is shared across tests)
        fr_mock.compare_faces.assert_called_with([[0.1]], [0.2], tolerance=0.4)


# ── find_faces ────────────────────────────────────────────────────────────────

class TestFindFaces:

    def _setup(self):
        util, vector_store = _make_util()
        return util, vector_store

    def test_returns_matched_name_when_face_recognised(self):
        util, vector_store = self._setup()
        fake_encoding = [0.1, 0.2]
        found_face = Face(id="1", person_name="Alice", embedding=[0.1, 0.2])
        vector_store.search.return_value = found_face

        with patch.object(util, "embed_face", return_value=fake_encoding), \
             patch.object(util, "compare_faces", return_value=True):

            result = util.find_faces(
                image=MagicMock(),
                unknown_faces_location=[(0, 10, 20, 30)],
                vector_store=vector_store,
            )

        assert result == ["Alice"]

    def test_returns_unknown_when_comparison_fails(self):
        util, vector_store = self._setup()
        fake_encoding = [0.9]
        found_face = Face(id="2", person_name="Bob", embedding=[0.1])
        vector_store.search.return_value = found_face

        with patch.object(util, "embed_face", return_value=fake_encoding), \
             patch.object(util, "compare_faces", return_value=False):

            result = util.find_faces(
                image=MagicMock(),
                unknown_faces_location=[(0, 10, 20, 30)],
                vector_store=vector_store,
            )

        assert result == ["Unknown"]

    def test_returns_unknown_when_vector_store_returns_none(self):
        util, vector_store = self._setup()
        vector_store.search.return_value = None

        with patch.object(util, "embed_face", return_value=[0.1]):
            result = util.find_faces(
                image=MagicMock(),
                unknown_faces_location=[(0, 10, 20, 30)],
                vector_store=vector_store,
            )

        assert result == ["Unknown"]

    def test_processes_multiple_faces(self):
        util, vector_store = self._setup()
        alice = Face(id="1", person_name="Alice", embedding=[0.1])
        bob = Face(id="2", person_name="Bob", embedding=[0.5])
        vector_store.search.side_effect = [alice, bob]

        with patch.object(util, "embed_face", side_effect=[[0.1], [0.5]]), \
             patch.object(util, "compare_faces", return_value=True):

            result = util.find_faces(
                image=MagicMock(),
                unknown_faces_location=[(0, 10, 20, 30), (40, 50, 60, 70)],
                vector_store=vector_store,
            )

        assert result == ["Alice", "Bob"]

    def test_returns_empty_list_when_no_face_locations(self):
        util, vector_store = self._setup()

        result = util.find_faces(
            image=MagicMock(),
            unknown_faces_location=[],
            vector_store=vector_store,
        )

        assert result == []
        vector_store.search.assert_not_called()

    def test_uses_tolerance_when_comparing(self):
        util, vector_store = self._setup()
        found_face = Face(id="1", person_name="Alice", embedding=[0.1])
        vector_store.search.return_value = found_face

        with patch.object(util, "embed_face", return_value=[0.1]), \
             patch.object(util, "compare_faces", return_value=True) as mock_compare:

            util.find_faces(
                image=MagicMock(),
                unknown_faces_location=[(0, 10, 20, 30)],
                vector_store=vector_store,
                tolerance=0.4,
            )

        mock_compare.assert_called_once_with(
            known_encoding=[0.1],
            unknown_encoding=[0.1],
            tolerance=0.4,
        )


# ── MTCNNRecognitionUtil.detect_faces ─────────────────────────────────────────

class TestMTCNNDetectFaces:

    def test_converts_mtcnn_boxes_to_face_recognition_format(self):
        vs = MagicMock()
        util = MTCNNRecognitionUtil(vector_store=vs)

        fake_result = [{"box": [10, 20, 50, 80]}]  # x, y, w, h
        with patch("utilities.face_recognition_utils.detector") as mock_detector:
            mock_detector.detect_faces.return_value = fake_result
            locations = util.detect_faces(image=MagicMock())

        # expected: (top, right, bottom, left) = (y, x+w, y+h, x) = (20, 60, 100, 10)
        assert locations == [(20, 60, 100, 10)]

    def test_returns_empty_list_when_no_faces(self):
        vs = MagicMock()
        util = MTCNNRecognitionUtil(vector_store=vs)

        with patch("utilities.face_recognition_utils.detector") as mock_detector:
            mock_detector.detect_faces.return_value = []
            locations = util.detect_faces(image=MagicMock())

        assert locations == []

    def test_handles_multiple_faces(self):
        vs = MagicMock()
        util = MTCNNRecognitionUtil(vector_store=vs)

        fake_result = [
            {"box": [0, 0, 10, 20]},
            {"box": [30, 40, 15, 25]},
        ]
        with patch("utilities.face_recognition_utils.detector") as mock_detector:
            mock_detector.detect_faces.return_value = fake_result
            locations = util.detect_faces(image=MagicMock())

        assert len(locations) == 2


# ── HogRecognitionUtil.detect_faces ──────────────────────────────────────────

class TestHogDetectFaces:

    def test_delegates_to_face_recognition_with_hog_model(self):
        util = HogRecognitionUtil(vector_store=MagicMock())
        fake_image = MagicMock()
        fr_mock.face_locations.return_value = [(10, 20, 30, 40)]

        result = util.detect_faces(fake_image)

        fr_mock.face_locations.assert_called_once_with(fake_image, model="hog")
        assert result == [(10, 20, 30, 40)]

    def test_returns_empty_list_when_no_faces(self):
        util = HogRecognitionUtil(vector_store=MagicMock())
        fr_mock.face_locations.return_value = []

        result = util.detect_faces(MagicMock())

        assert result == []


# ── CNNRecognitionUtil.detect_faces ──────────────────────────────────────────

class TestCNNDetectFaces:

    def test_delegates_to_face_recognition_with_cnn_model(self):
        util = CNNRecognitionUtil(vector_store=MagicMock())
        fake_image = MagicMock()
        fr_mock.face_locations.return_value = [(5, 15, 25, 35)]

        result = util.detect_faces(fake_image)

        fr_mock.face_locations.assert_called_with(fake_image, model="cnn")
        assert result == [(5, 15, 25, 35)]
