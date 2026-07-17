from __future__ import annotations

import argparse
import csv
import json
import re
import sys
import time
import webbrowser
from datetime import datetime
from pathlib import Path
from typing import Callable

PROJECT_ROOT = Path(__file__).resolve().parent.parent
if str(PROJECT_ROOT) not in sys.path:
    sys.path.insert(0, str(PROJECT_ROOT))

import cv2 
import numpy as np  

IMAGE_EXTS = {".jpg", ".jpeg", ".png", ".bmp", ".webp"}
FILENAME_COUNT_RE = re.compile(r"(\d+)(?!.*\d)")  # ultimul numar din nume


class HaarCounter:
    """Numarare identica cu cea de pe Raspberry Pi (app.py -> count_faces)."""

    name = "Haar"

    def __init__(self, detection_width: int = 320):
        cascade_path = cv2.data.haarcascades + "haarcascade_frontalface_default.xml"
        self._detector = cv2.CascadeClassifier(cascade_path)
        if self._detector.empty():
            raise RuntimeError(f"Nu s-a putut incarca cascada Haar din {cascade_path}")
        self.detection_width = detection_width

    def count(self, image_rgb: np.ndarray) -> int:
        gray = cv2.cvtColor(image_rgb, cv2.COLOR_RGB2GRAY)
        h, w = gray.shape[:2]
        if self.detection_width and w > self.detection_width:
            scale = self.detection_width / w
            gray = cv2.resize(gray, (self.detection_width, int(h * scale)),
                              interpolation=cv2.INTER_AREA)
        gray = cv2.equalizeHist(gray)
        faces = self._detector.detectMultiScale(
            gray, scaleFactor=1.1, minNeighbors=5, minSize=(40, 40),
        )
        return len(faces)


class UtilCounter:

    def __init__(self, name: str, util_cls):
        self.name = name
        self._util = util_cls(vector_store=None)

    def count(self, image_rgb: np.ndarray) -> int:
        return len(self._util.detect_faces(image_rgb))


def build_detectors(names: list[str]) -> list:
    from utilities.face_recognition_utils import (
        HogRecognitionUtil, CNNRecognitionUtil, MTCNNRecognitionUtil,
    )
    factory: dict[str, Callable] = {
        "Haar":  lambda: HaarCounter(),
        "HOG":   lambda: UtilCounter("HOG", HogRecognitionUtil),
        "MTCNN": lambda: UtilCounter("MTCNN", MTCNNRecognitionUtil),
        "CNN":   lambda: UtilCounter("CNN", CNNRecognitionUtil),
    }
    detectors = []
    for n in names:
        if n not in factory:
            print(f"  [WARN] detector necunoscut, ignorat: {n}")
            continue
        detectors.append(factory[n]())
    return detectors


def load_labels(dataset: Path) -> dict[str, int]:
    """Incearca labels.csv, apoi labels.json, apoi numarul din numele fisierului."""
    labels: dict[str, int] = {}

    csv_path = dataset / "labels.csv"
    if csv_path.exists():
        with csv_path.open(newline="", encoding="utf-8") as fh:
            for row in csv.reader(fh):
                if len(row) < 2 or row[0].strip().lower() in {"filename", "file", "image"}:
                    continue
                try:
                    labels[row[0].strip()] = int(row[1])
                except ValueError:
                    continue
        if labels:
            return labels

    json_path = dataset / "labels.json"
    if json_path.exists():
        data = json.loads(json_path.read_text(encoding="utf-8"))
        return {k: int(v) for k, v in data.items()}

    return labels  # gol -> se va incerca din numele fisierului


def collect_images(dataset: Path, labels: dict[str, int]) -> list[tuple[Path, int]]:
    items: list[tuple[Path, int]] = []
    for p in sorted(dataset.iterdir()):
        if p.suffix.lower() not in IMAGE_EXTS:
            continue
        if p.name in labels:
            true_count = labels[p.name]
        else:
            m = FILENAME_COUNT_RE.search(p.stem)
            if not m:
                print(f"  [WARN] fara eticheta si fara numar in nume, sarit: {p.name}")
                continue
            true_count = int(m.group(1))
        items.append((p, true_count))
    return items


def load_rgb(path: Path) -> np.ndarray | None:
    bgr = cv2.imread(str(path))
    if bgr is None:
        return None
    return cv2.cvtColor(bgr, cv2.COLOR_BGR2RGB)


def counting_metrics(trues: list[int], preds: list[int]) -> dict:
    n = len(trues)
    tp = sum(min(t, p) for t, p in zip(trues, preds))
    fp = sum(max(0, p - t) for t, p in zip(trues, preds))
    fn = sum(max(0, t - p) for t, p in zip(trues, preds))

    precision = tp / (tp + fp) if (tp + fp) else 0.0
    recall    = tp / (tp + fn) if (tp + fn) else 0.0
    f1        = 2 * precision * recall / (precision + recall) if (precision + recall) else 0.0

    exact   = sum(1 for t, p in zip(trues, preds) if t == p) / n if n else 0.0

    return {
        "images": n,
        "accuracy": exact,          # rata de nimereala exacta a numarului
        "precision": precision,
        "recall": recall,
        "f1": f1,
        "tp": tp, "fp": fp, "fn": fn,
        "total_true": sum(trues),
        "total_pred": sum(preds),
    }


def evaluate(items: list[tuple[Path, int]], detector_names: list[str],
             dataset_label: str) -> dict:
    if not items:
        raise SystemExit(
            "Niciun fisier valid. Adauga poze + labels.csv "
            "(filename,count) sau pune numarul in numele fisierului."
        )

    detectors = build_detectors(detector_names)
    if not detectors:
        raise SystemExit("Niciun detector valid selectat.")

    print(f"Dataset : {dataset_label}  ({len(items)} poze)")
    print(f"Detectoare: {', '.join(d.name for d in detectors)}\n")

    # thumbnail-uri + true count, o singura data (partajate intre detectoare)
    images_meta: dict[str, dict] = {}
    loaded: list[tuple[str, np.ndarray, int]] = []
    for path, true_count in items:
        img = load_rgb(path)
        if img is None:
            print(f"  [WARN] nu s-a putut citi: {path.name}")
            continue
        images_meta[path.name] = {"true": true_count}
        loaded.append((path.name, img, true_count))

    detectors_out: dict[str, dict] = {}
    for det in detectors:
        print(f"→ {det.name}")
        trues, preds, per_image = [], [], []
        for fname, img, true_count in loaded:
            t0 = time.perf_counter()
            pred = det.count(img)
            dt = (time.perf_counter() - t0) * 1000
            diff = pred - true_count
            status = "exact" if diff == 0 else ("over" if diff > 0 else "under")
            trues.append(true_count)
            preds.append(pred)
            per_image.append({
                "file": fname, "true": true_count, "pred": pred,
                "diff": diff, "status": status, "time_ms": round(dt, 1),
            })
            mark = "OK " if diff == 0 else f"{diff:+d} "
            print(f"   {mark}{fname}: real={true_count} detectat={pred}  ({dt:.0f} ms)")

        metrics = counting_metrics(trues, preds)
        metrics["avg_time_ms"] = round(sum(pi["time_ms"] for pi in per_image) / len(per_image), 1) if per_image else 0
        metrics["n_exact"] = sum(1 for pi in per_image if pi["status"] == "exact")
        metrics["n_over"]  = sum(1 for pi in per_image if pi["status"] == "over")
        metrics["n_under"] = sum(1 for pi in per_image if pi["status"] == "under")
        detectors_out[det.name] = {"metrics": metrics, "per_image": per_image}
        print(f"   → accuracy={metrics['accuracy']:.3f}  F1={metrics['f1']:.3f}\n")

    best = max(detectors_out, key=lambda d: detectors_out[d]["metrics"]["f1"])
    return {
        "generated_at": datetime.now().strftime("%Y-%m-%d %H:%M:%S"),
        "dataset_dir": dataset_label,
        "best_detector": best,
        "images_meta": images_meta,
        "detectors": detectors_out,
    }


def render_html(results: dict) -> str:
    payload = json.dumps(results).replace("</", "<\\/")
    return _HTML_TEMPLATE.replace("__DATA__", payload)


_HTML_TEMPLATE = r"""<!DOCTYPE html>
<html lang="ro">
<head>
<meta charset="utf-8"/>
<meta name="viewport" content="width=device-width, initial-scale=1"/>
<title>RoomVision — evaluare numarare persoane</title>
<style>
  :root{
    --bg:#ffffff; --card:#ffffff; --card2:#f6f7f9; --line:#e3e6ea;
    --txt:#1a1d23; --muted:#5f6875; --accent:#2f6bff;
    --ok:#1f9d63; --over:#d98319; --under:#d1452a;
  }
  *{box-sizing:border-box}
  body{margin:0;background:var(--bg);color:var(--txt);
    font-family:-apple-system,Segoe UI,Roboto,Helvetica,Arial,sans-serif;line-height:1.5}
  .wrap{max-width:1100px;margin:0 auto;padding:28px 20px 60px}
  h1{font-size:22px;font-weight:600;margin:0 0 4px}
  .sub{color:var(--muted);font-size:13px;margin-bottom:22px}
  .best{font-size:11px;background:var(--accent);color:#fff;padding:2px 8px;border-radius:6px}
  .detsec{margin-top:34px;padding-top:22px;border-top:1px solid var(--line)}
  .dethead{display:flex;align-items:center;gap:10px;margin-bottom:14px;flex-wrap:wrap}
  .dethead h2{font-size:18px;font-weight:600;margin:0}
  .dstat{color:var(--muted);font-size:13px}
  .empty{color:var(--muted);font-size:13px;padding:8px 0}
  .cards{display:grid;grid-template-columns:repeat(auto-fit,minmax(150px,1fr));gap:12px;margin-bottom:22px}
  .metric{background:var(--card);border:1px solid var(--line);border-radius:12px;padding:16px}
  .metric .lbl{font-size:12px;color:var(--muted);text-transform:uppercase;letter-spacing:.04em}
  .metric .val{font-size:28px;font-weight:600;margin-top:6px}
  .metric .hint{font-size:12px;color:var(--muted);margin-top:2px}
  .brk{font-size:14px;color:var(--muted);margin-top:14px}
  .brk b{color:var(--txt);font-weight:600}
  .brk .ok{color:var(--ok)} .brk .over{color:var(--over)} .brk .under{color:var(--under)}
  .cmp{width:100%;border-collapse:collapse;margin-top:8px;font-size:14px}
  .cmp th,.cmp td{text-align:left;padding:8px 10px;border-bottom:1px solid var(--line)}
  .cmp th{color:var(--muted);font-weight:500;font-size:12px;text-transform:uppercase}
  .cmp td.n{text-align:right;font-variant-numeric:tabular-nums}
  details{margin-top:26px}
  summary{cursor:pointer;color:var(--muted);font-size:14px}
</style>
</head>
<body>
<div class="wrap">
  <h1>Evaluare numarare persoane in incaperi</h1>

  <details open>
    <summary>Comparatie intre detectoare</summary>
    <table class="cmp" id="cmp"></table>
  </details>

  <div id="sections"></div>
</div>

<script>
const DATA = __DATA__;
const pct = v => (v*100).toFixed(1) + "%";
const fnum = v => (Math.round(v*1000)/1000).toFixed(3);

function card(lbl, val, hint){
  return `<div class="metric"><div class="lbl">${lbl}</div><div class="val">${val}</div>`+
         (hint?`<div class="hint">${hint}</div>`:'')+`</div>`;
}

function renderCmp(){
  const rows = Object.entries(DATA.detectors).map(([name,d])=>{
    const m = d.metrics;
    const star = name===DATA.best_detector ? " ★" : "";
    return `<tr>
      <td>${name}${star}</td>
      <td class="n">${pct(m.accuracy)}</td>
      <td class="n">${fnum(m.precision)}</td>
      <td class="n">${fnum(m.recall)}</td>
      <td class="n">${fnum(m.f1)}</td>
      <td class="n">${m.avg_time_ms} ms</td>
    </tr>`;
  }).join("");
  document.getElementById("cmp").innerHTML =
    `<tr><th>Detector</th><th style="text-align:right">Accuracy</th>
     <th style="text-align:right">Precision</th><th style="text-align:right">Recall</th>
     <th style="text-align:right">F1</th>
     <th style="text-align:right">Viteza medie</th></tr>` + rows;
}

function renderSections(){
  const el = document.getElementById("sections"); el.innerHTML = "";
  for(const [name, d] of Object.entries(DATA.detectors)){
    const m = d.metrics;
    const best = name===DATA.best_detector ? '<span class="best">cel mai bun F1</span>' : '';
    const cards =
      card("Accuracy", pct(m.accuracy), "numar exact nimerit") +
      card("Precision", fnum(m.precision), "din persoanele detectate") +
      card("Recall", fnum(m.recall), "din persoanele reale") +
      card("F1 score", fnum(m.f1)) +
      card("Viteza medie", `${m.avg_time_ms} ms`, "timp mediu pe poza");
    el.insertAdjacentHTML("beforeend", `
      <section class="detsec">
        <div class="dethead">
          <h2>${name}</h2>${best}
        </div>
        <div class="cards">${cards}</div>
      </section>`);
  }
}

renderCmp(); renderSections();
</script>
</body>
</html>
"""



def main():
    ap = argparse.ArgumentParser(description="Evaluare numarare persoane in incaperi")
    ap.add_argument("--dataset", default=str(Path(__file__).parent / "dataset_rooms"),
                    help="director cu pozele din sali")
    ap.add_argument("--detectors", default="Haar,HOG,MTCNN",
                    help="lista separata prin virgula: Haar,HOG,MTCNN,CNN")
    ap.add_argument("--out", default=str(Path(__file__).parent / "report.html"),
                    help="fisierul HTML de iesire")
    ap.add_argument("--json", default="", help="salveaza si results.json (optional)")
    ap.add_argument("--open", action="store_true", help="deschide raportul in browser")
    ap.add_argument("--limit", type=int, default=0,
                    help="evalueaza doar primele N poze (util pentru dataset mare / graba)")
    ap.add_argument("--wider-gt", default="",
                    help="fisier anotari WIDER FACE (ex: wider_face_val_bbx_gt.txt)")
    ap.add_argument("--wider-images", default="",
                    help="director imagini WIDER FACE (ex: WIDER_val/images)")
    args = ap.parse_args()

    names = [n.strip() for n in args.detectors.split(",") if n.strip()]

    if args.wider_gt:
        if not args.wider_images:
            raise SystemExit("--wider-gt necesita si --wider-images")
        gt_path = Path(args.wider_gt)
        img_dir = Path(args.wider_images)
        if not gt_path.exists():
            raise SystemExit(
                f"Nu gasesc fisierul de anotari: {gt_path}\n"
                f"  (calea e relativa la directorul curent: {Path.cwd()})\n"
                f"  Descarca 'wider_face_split.zip' de pe http://shuoyang1213.me/WIDERFACE/ ,\n"
                f"  dezarhiveaza-l, apoi da calea completa catre wider_face_val_bbx_gt.txt."
            )
        if not img_dir.exists():
            raise SystemExit(
                f"Nu gasesc directorul de imagini: {img_dir}\n"
                f"  Descarca 'WIDER_val.zip', dezarhiveaza si arata catre WIDER_val/images"
            )
        items = collect_wider(gt_path, img_dir, args.limit)
        if not items:
            raise SystemExit(
                f"Am citit anotarile dar nicio imagine nu a fost gasita in {img_dir}.\n"
                f"  Verifica structura: {img_dir}/<eveniment>/<poza>.jpg"
            )
        dataset_label = f"WIDER FACE — {gt_path.name}"
    else:
        dataset = Path(args.dataset)
        labels = load_labels(dataset)
        items = collect_images(dataset, labels)
        if args.limit:
            items = items[:args.limit]
        dataset_label = str(dataset)

    results = evaluate(items, names, dataset_label)

    out = Path(args.out)
    out.write_text(render_html(results), encoding="utf-8")
    print(f"Raport HTML: {out.resolve()}")

    if args.json:
        Path(args.json).write_text(json.dumps(results, indent=2), encoding="utf-8")
        print(f"JSON       : {Path(args.json).resolve()}")

    best = results["best_detector"]
    bm = results["detectors"][best]["metrics"]
    print(f"\nCel mai bun detector (F1): {best}  "
          f"accuracy={bm['accuracy']:.3f}  F1={bm['f1']:.3f}")

    if args.open:
        webbrowser.open(out.resolve().as_uri())


if __name__ == "__main__":
    main()
