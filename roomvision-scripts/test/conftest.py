"""
Patch heavy ML/CV dependencies at sys.modules level before any project
modules are imported.  This keeps tests fast and free of GPU/model-file
requirements while still exercising all service logic.
"""
import sys
from pathlib import Path
from unittest.mock import MagicMock

# ── stub every heavy dependency ───────────────────────────────────────────────
_HEAVY = [
    # face recognition / ML
    "face_recognition",
    "mtcnn",
    "mtcnn.MTCNN",
    "tensorflow",
    "mediapipe",
    # image / numeric
    "cv2",
    "numpy",
    "numpy.linalg",
    "matplotlib",
    "matplotlib.image",
    # qdrant
    "qdrant_client",
    "qdrant_client.http",
    "qdrant_client.http.models",
    # dotenv
    "dotenv",
]
for _mod in _HEAVY:
    if _mod not in sys.modules:
        sys.modules[_mod] = MagicMock()

# python-dotenv: make load_dotenv a no-op
sys.modules["dotenv"].load_dotenv = MagicMock()

# Make matplotlib submodule accessible via the parent mock
sys.modules["matplotlib"].image = sys.modules["matplotlib.image"]

# ── add project root so imports resolve ──────────────────────────────────────
PROJECT_ROOT = Path(__file__).parent.parent
if str(PROJECT_ROOT) not in sys.path:
    sys.path.insert(0, str(PROJECT_ROOT))
