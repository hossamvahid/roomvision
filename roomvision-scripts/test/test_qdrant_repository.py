"""Unit tests for QdrantRepository."""
import sys
from unittest.mock import MagicMock, patch, call
import pytest

# conftest.py patches qdrant_client and dotenv before import
from data_access.qdrant_repository import QdrantRepository
from data_access.face import Face


def _make_repo():
    """Return a QdrantRepository with a mocked Qdrant client."""
    with patch("data_access.qdrant_repository.init_qdrant") as mock_init, \
         patch.dict("os.environ", {"COLLECTION_NAME": "test_collection"}):
        mock_client = MagicMock()
        mock_init.return_value = mock_client
        repo = QdrantRepository()
    return repo, mock_client


class TestCreatePayload:

    def test_returns_dict_with_name_key(self):
        repo, _ = _make_repo()
        payload = repo.create_payload(person_name="Alice")
        assert payload == {"name": "Alice"}

    def test_returns_correct_name_for_different_persons(self):
        repo, _ = _make_repo()
        assert repo.create_payload(person_name="Bob") == {"name": "Bob"}
        assert repo.create_payload(person_name="Carol") == {"name": "Carol"}


class TestAddVector:

    def test_calls_client_upsert_with_correct_data(self):
        repo, client = _make_repo()
        vector = [0.1, 0.2, 0.3]
        payload = {"name": "Alice"}

        repo.add_vector(point_id="uuid-1", vector=vector, payload=payload)

        client.upsert.assert_called_once()
        call_kwargs = client.upsert.call_args.kwargs
        assert call_kwargs["collection_name"] == "test_collection"
        point = call_kwargs["points"][0]
        assert point["id"] == "uuid-1"
        assert point["vector"] == vector
        assert point["payload"] == payload

    def test_upserts_to_configured_collection(self):
        repo, client = _make_repo()
        repo.add_vector(point_id="id", vector=[0.5], payload=None)

        call_kwargs = client.upsert.call_args.kwargs
        assert call_kwargs["collection_name"] == "test_collection"

    def test_accepts_none_payload(self):
        repo, client = _make_repo()
        repo.add_vector(point_id="id-2", vector=[0.1], payload=None)
        client.upsert.assert_called_once()


class TestSearch:

    def _make_mock_point(self, point_id, name, vector):
        point = MagicMock()
        point.id = point_id
        point.payload = {"name": name}
        point.vector = vector
        return point

    def test_returns_face_with_correct_data(self):
        repo, client = _make_repo()
        mock_point = self._make_mock_point("abc", "Alice", [0.1, 0.2])
        client.query_points.return_value.points = [mock_point]

        result = repo.search(vector=[0.1, 0.2])

        assert isinstance(result, Face)
        assert result.getId() == "abc"
        assert result.getPersonName() == "Alice"
        assert result.getEmbedding() == [0.1, 0.2]

    def test_searches_in_configured_collection(self):
        repo, client = _make_repo()
        mock_point = self._make_mock_point("x", "Bob", [0.5])
        client.query_points.return_value.points = [mock_point]

        repo.search(vector=[0.5])

        call_kwargs = client.query_points.call_args.kwargs
        assert call_kwargs["collection_name"] == "test_collection"

    def test_uses_default_limit_of_five(self):
        repo, client = _make_repo()
        mock_point = self._make_mock_point("y", "Carol", [0.3])
        client.query_points.return_value.points = [mock_point]

        repo.search(vector=[0.3])

        call_kwargs = client.query_points.call_args.kwargs
        assert call_kwargs["limit"] == 5

    def test_requests_vectors_and_payload(self):
        repo, client = _make_repo()
        mock_point = self._make_mock_point("z", "Dave", [0.7])
        client.query_points.return_value.points = [mock_point]

        repo.search(vector=[0.7])

        call_kwargs = client.query_points.call_args.kwargs
        assert call_kwargs["with_vectors"] is True
        assert call_kwargs["with_payload"] is True

    def test_passes_query_vector_correctly(self):
        repo, client = _make_repo()
        query = [0.1, 0.2, 0.3]
        mock_point = self._make_mock_point("q", "Eve", query)
        client.query_points.return_value.points = [mock_point]

        repo.search(vector=query)

        call_kwargs = client.query_points.call_args.kwargs
        assert call_kwargs["query"] == query
