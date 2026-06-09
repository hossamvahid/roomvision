"""Unit tests for RecognitionService."""
import sys
from unittest.mock import MagicMock, patch
import pytest

# conftest.py already patched heavy deps and added project root to sys.path
from services.recognition_service import RecognitionService
from data_access.interface import VectorStore
from utilities.face_recognition_utils import RecognitionUtil


# ── helpers ───────────────────────────────────────────────────────────────────

def _make_service():
    vector_store = MagicMock(spec=VectorStore)
    recognition_util = MagicMock(spec=RecognitionUtil)
    service = RecognitionService(vector_store=vector_store, recognition_util=recognition_util)
    return service, vector_store, recognition_util


DUMMY_IMAGE_BYTES = b"\xff\xd8\xff\xe0"  # minimal JPEG header


# ── EmbedFace ─────────────────────────────────────────────────────────────────

class TestEmbedFace:

    def test_returns_embedding_when_face_detected(self):
        service, vector_store, recognition_util = _make_service()

        fake_embedding = [0.1, 0.2, 0.3]
        recognition_util.embed_face.return_value = fake_embedding
        vector_store.create_payload.return_value = {"name": "Alice"}

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = MagicMock()
            result = service.EmbedFace(DUMMY_IMAGE_BYTES, "Alice")

        assert result == fake_embedding
        vector_store.add_vector.assert_called_once()
        call_kwargs = vector_store.add_vector.call_args.kwargs
        assert call_kwargs["vector"] == fake_embedding
        assert call_kwargs["payload"] == {"name": "Alice"}

    def test_returns_none_when_no_face_detected(self):
        service, vector_store, recognition_util = _make_service()
        recognition_util.embed_face.return_value = None

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = MagicMock()
            result = service.EmbedFace(DUMMY_IMAGE_BYTES, "Alice")

        assert result is None
        vector_store.add_vector.assert_not_called()

    def test_stores_vector_with_generated_uuid(self):
        service, vector_store, recognition_util = _make_service()
        recognition_util.embed_face.return_value = [0.5]
        vector_store.create_payload.return_value = {"name": "Bob"}

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = MagicMock()
            with patch("services.recognition_service.uuid4") as mock_uuid:
                mock_uuid.return_value = "fixed-uuid"
                service.EmbedFace(DUMMY_IMAGE_BYTES, "Bob")

        call_kwargs = vector_store.add_vector.call_args.kwargs
        assert call_kwargs["point_id"] == "fixed-uuid"

    def test_creates_payload_with_person_name(self):
        service, vector_store, recognition_util = _make_service()
        recognition_util.embed_face.return_value = [0.1]

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = MagicMock()
            service.EmbedFace(DUMMY_IMAGE_BYTES, "Carol")

        vector_store.create_payload.assert_called_once_with(person_name="Carol")

    def test_converts_bytes_to_image_before_embedding(self):
        service, vector_store, recognition_util = _make_service()
        fake_image = MagicMock()
        recognition_util.embed_face.return_value = None

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = fake_image
            service.EmbedFace(DUMMY_IMAGE_BYTES, "Dave")

        mock_bytes.bytes_to_image.assert_called_once_with(DUMMY_IMAGE_BYTES)
        recognition_util.embed_face.assert_called_once_with(fake_image)


# ── VerifyRoom ────────────────────────────────────────────────────────────────

class TestVerifyRoom:

    def test_returns_matched_persons_when_faces_found(self):
        service, vector_store, recognition_util = _make_service()
        fake_image = MagicMock()
        recognition_util.detect_faces.return_value = [(10, 20, 30, 40), (50, 60, 70, 80)]
        recognition_util.find_faces.return_value = ["Alice", "Bob"]

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = fake_image
            result = service.VerifyRoom(DUMMY_IMAGE_BYTES)

        assert result == ["Alice", "Bob"]

    def test_returns_empty_list_when_no_faces_detected(self):
        service, vector_store, recognition_util = _make_service()
        recognition_util.detect_faces.return_value = []

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = MagicMock()
            result = service.VerifyRoom(DUMMY_IMAGE_BYTES)

        assert result == []
        recognition_util.find_faces.assert_not_called()

    def test_passes_correct_arguments_to_find_faces(self):
        service, vector_store, recognition_util = _make_service()
        fake_image = MagicMock()
        face_locations = [(0, 10, 20, 30)]
        recognition_util.detect_faces.return_value = face_locations
        recognition_util.find_faces.return_value = ["Alice"]

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = fake_image
            service.VerifyRoom(DUMMY_IMAGE_BYTES)

        recognition_util.find_faces.assert_called_once_with(
            image=fake_image,
            unknown_faces_location=face_locations,
            vector_store=vector_store,
            tolerance=0.5,
        )

    def test_returns_unknown_for_unrecognised_faces(self):
        service, vector_store, recognition_util = _make_service()
        recognition_util.detect_faces.return_value = [(0, 10, 20, 30)]
        recognition_util.find_faces.return_value = ["Unknown"]

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = MagicMock()
            result = service.VerifyRoom(DUMMY_IMAGE_BYTES)

        assert result == ["Unknown"]

    def test_converts_bytes_to_image_before_detection(self):
        service, vector_store, recognition_util = _make_service()
        fake_image = MagicMock()
        recognition_util.detect_faces.return_value = []

        with patch("services.recognition_service.bytes_utils") as mock_bytes:
            mock_bytes.bytes_to_image.return_value = fake_image
            service.VerifyRoom(DUMMY_IMAGE_BYTES)

        mock_bytes.bytes_to_image.assert_called_once_with(DUMMY_IMAGE_BYTES)
        recognition_util.detect_faces.assert_called_once_with(image=fake_image)
