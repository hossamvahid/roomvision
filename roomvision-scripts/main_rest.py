import uvicorn
from fastapi import FastAPI
from dotenv import load_dotenv
import os

from logs.logging_config import setup_logging, set_correlation_id
from transport.rest.middlewares.correlation_id_middleware import CorrelationIdMiddleware
from transport.rest.routes.face_recognition_router import create_router
from data_access.qdrant_repository import QdrantRepository
from services.recognition_service import RecognitionService
from utilities.face_recognition_utils import MTCNNRecognitionUtil,CNNRecognitionUtil,HogRecognitionUtil

load_dotenv()

logger = setup_logging()
set_correlation_id()

app = FastAPI(title="RoomVision Face Recognition API")
app.add_middleware(CorrelationIdMiddleware)

vector_store = QdrantRepository()
recognition_util = MTCNNRecognitionUtil(vector_store=vector_store)
recognition_service = RecognitionService(vector_store=vector_store, recognition_util=recognition_util)

app.include_router(create_router(recognition_service), prefix="/api")

if __name__ == "__main__":
    rest_port = int(os.getenv("REST_PORT"))
    url = os.getenv("URL")
    logger.info(f"Server has started on port {rest_port}.")
    uvicorn.run("transport.rest.main:app", host=url, port=rest_port, reload=False)
