from qdrant_client import QdrantClient
from qdrant_client.http.models import VectorParams, Distance
from dotenv import load_dotenv
import os

load_dotenv()

def init_qdrant():
    url = os.getenv("QDRANT_URL")
    collection_name = os.getenv("COLLECTION_NAME")
    vector_size = int(os.getenv("VECTOR_DIM"))
    distance_metric = os.getenv("DISTANCE")

    client = QdrantClient(url=url)
    if client.collection_exists(collection_name=collection_name) is False:
        client.create_collection(
            collection_name=collection_name,
            vectors_config=VectorParams(size=vector_size, distance=Distance[distance_metric.upper()])
        )
    return client
