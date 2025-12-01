from .client import init_qdrant
from .interface import VectorStore
from dotenv import load_dotenv
import os
from logs.logging_config import get_logger
from .face import Face

class QdrantRepository(VectorStore):

    def __init__(self):
        self.collection_name = os.getenv("COLLECTION_NAME")
        self.client = init_qdrant()

    def add_vector(self, point_id, vector, payload=None):
        logger = get_logger()
        logger.info(f"Adding vector with ID {point_id} and {payload}")

        self.client.upsert(
            collection_name=self.collection_name,
            points=[
                {
                    "id": point_id,
                    "vector": vector,
                    "payload": payload
                }
            ])
        
    def search(self, vector, limit=5):
        result = self.client.query_points(
            collection_name=self.collection_name,
            query=vector,
            limit=limit,
            with_vectors=True,
            with_payload=True
        )

        result_id = result.points[0].id
        result_name = result.points[0].payload["name"]
        result_vector = result.points[0].vector
        return Face(result_id, result_name, result_vector)
        
    
    def create_payload(self, person_name):
        return {"name": person_name}