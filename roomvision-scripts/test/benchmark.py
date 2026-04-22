"""
Directory structure expected:
  <photos_dir>/
    image1.jpg       <- photo file (.jpg, .jpeg, .png, .bmp, .webp)
    image1.txt       <- single integer: expected number of persons
    ...
"""

import argparse
import os
import sys
import time
from dataclasses import dataclass, field
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent.parent))

import requests
import grpc
from transport.grpc.protos import face_recognition_pb2, face_recognition_pb2_grpc

IMAGE_EXTENSIONS = {".jpg", ".jpeg", ".png", ".bmp", ".webp"}


@dataclass
class RunResult:
    expected: int
    predicted: int
    latency_ms: float
    success: bool
    error: str = ""

    @property
    def correct(self) -> bool:
        return self.success and self.predicted == self.expected


@dataclass
class BenchmarkResult:
    transport: str
    runs: list[RunResult] = field(default_factory=list)

    @property
    def total(self) -> int:
        return len(self.runs)

    @property
    def successful(self) -> int:
        return sum(1 for r in self.runs if r.success)

    @property
    def accurate(self) -> int:
        return sum(1 for r in self.runs if r.correct)

    @property
    def accuracy_pct(self) -> float:
        return (self.accurate / self.total * 100) if self.total else 0.0

    def _latencies(self) -> list[float]:
        return [r.latency_ms for r in self.runs if r.success]

    @property
    def avg_latency_ms(self) -> float:
        lats = self._latencies()
        return sum(lats) / len(lats) if lats else 0.0

    @property
    def min_latency_ms(self) -> float:
        lats = self._latencies()
        return min(lats) if lats else 0.0

    @property
    def max_latency_ms(self) -> float:
        lats = self._latencies()
        return max(lats) if lats else 0.0


def load_test_cases(directory: str) -> list[tuple[Path, int]]:
    cases = []
    dir_path = Path(directory)
    if not dir_path.is_dir():
        print(f"[ERROR] Directory not found: {directory}")
        sys.exit(1)

    for img_path in sorted(dir_path.iterdir()):
        if img_path.suffix.lower() not in IMAGE_EXTENSIONS:
            continue
        txt_path = img_path.with_suffix(".txt")
        if not txt_path.exists():
            print(f"[WARN] No label file for {img_path.name}, skipping.")
            continue
        try:
            expected = int(txt_path.read_text().strip())
        except ValueError:
            print(f"[WARN] Invalid label in {txt_path.name}, skipping.")
            continue
        cases.append((img_path, expected))

    if not cases:
        print("[ERROR] No valid image/label pairs found.")
        sys.exit(1)

    return cases


def _rest_call(url: str, img_path: Path, expected: int) -> RunResult:
    image_bytes = img_path.read_bytes()
    try:
        start = time.perf_counter()
        resp = requests.post(
            url,
            files={"image": (img_path.name, image_bytes, "image/jpeg")},
            timeout=30,
        )
        latency_ms = (time.perf_counter() - start) * 1000

        if resp.status_code == 200:
            data = resp.json()
            return RunResult(expected=expected, predicted=data.get("total", 0),
                             latency_ms=latency_ms, success=True)
        return RunResult(expected=expected, predicted=-1, latency_ms=latency_ms,
                         success=False, error=f"HTTP {resp.status_code}: {resp.text[:120]}")
    except Exception as exc:
        return RunResult(expected=expected, predicted=-1, latency_ms=0.0,
                         success=False, error=str(exc))


def benchmark(cases: list[tuple[Path, int]], base_url: str, iterations: int) -> BenchmarkResult:
    result = BenchmarkResult(transport="REST")
    url = f"{base_url.rstrip('/')}/api/verify-room"

    for iteration in range(1, iterations + 1):
        print(f"iteration {iteration}/{iterations} ...", end="\r")
        for img_path, expected in cases:
            result.runs.append(_rest_call(url, img_path, expected))

    print()
    return result


def print_summary(bench: BenchmarkResult) -> None:
    total_runs = bench.total
    images = total_runs
    print(f"  Runs      : {total_runs}  (successful: {bench.successful})")
    print(f"  Accuracy  : {bench.accuracy_pct:.1f}%  ({bench.accurate}/{total_runs} correct)")
    if bench.successful:
        print(f"  Latency   : avg={bench.avg_latency_ms:.1f}ms  "
              f"min={bench.min_latency_ms:.1f}ms  "
              f"max={bench.max_latency_ms:.1f}ms")
    else:
        print("  Latency   : no successful runs")

    errors = [(r.error) for r in bench.runs if not r.success]
    if errors:
        unique_errors = dict.fromkeys(errors)
        print(f"  Errors ({len(errors)}):")
        for err in unique_errors:
            print(f"    • {err}")

def main() -> None:
    parser = argparse.ArgumentParser(
        description="Benchmark REST vs gRPC transport for face recognition."
    )
    parser.add_argument("--dir", required=True,
                        help="Directory containing images and .txt label files.")
    parser.add_argument("--iterations", type=int, default=1,
                        help="Number of times to repeat the full image set (default: 1).")
    parser.add_argument("--rest-url", default="http://localhost:8000",
                        help="REST base URL (default: http://localhost:8000).")
    args = parser.parse_args()

    if args.iterations < 1:
        print("[ERROR] --iterations must be >= 1")
        sys.exit(1)

    cases = load_test_cases(args.dir)
    print(f"\nLoaded {len(cases)} image(s) — {args.iterations} iteration(s) each "
          f"= {len(cases) * args.iterations} total request(s) per transport.")

    all_results: list[BenchmarkResult] = []

    print(f"\nRunning benchmark  →  {args.rest_url}")
    all_results.append(benchmark(cases, args.rest_url, args.iterations))

    print(f"\n{'='*62}")
    print("  RESULTS")
    print(f"{'='*62}")
    for r in all_results:
        print_summary(r)

   


if __name__ == "__main__":
    main()
