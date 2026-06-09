"""Unit tests for the Face data class."""
from data_access.face import Face


class TestFace:

    def test_stores_all_attributes_on_construction(self):
        face = Face(id="abc-123", person_name="Alice", embedding=[0.1, 0.2, 0.3])

        assert face.getId() == "abc-123"
        assert face.getPersonName() == "Alice"
        assert face.getEmbedding() == [0.1, 0.2, 0.3]

    def test_get_person_name_returns_correct_value(self):
        face = Face(id="x", person_name="Bob", embedding=[])
        assert face.getPersonName() == "Bob"

    def test_get_embedding_returns_correct_list(self):
        embedding = [0.5, 0.6, 0.7, 0.8]
        face = Face(id="y", person_name="Carol", embedding=embedding)
        assert face.getEmbedding() == embedding

    def test_get_id_returns_correct_value(self):
        face = Face(id="unique-id-999", person_name="Dave", embedding=[])
        assert face.getId() == "unique-id-999"

    def test_accepts_empty_embedding(self):
        face = Face(id="z", person_name="Eve", embedding=[])
        assert face.getEmbedding() == []

    def test_accepts_high_dimensional_embedding(self):
        embedding = list(range(128))
        face = Face(id="w", person_name="Frank", embedding=embedding)
        assert len(face.getEmbedding()) == 128
