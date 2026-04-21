import time
import grpc
from logs.logging_config import setup_logging, set_correlation_id
from concurrent import futures
from services.recognition_service import RecognitionService
from transport.grpc.interceptors.correlation_id_interceptor import CorrelationIdInterceptor
from transport.grpc.interceptors.end_request_interceptor import EndRequestInterceptor
from transport.grpc.protos import face_recognition_pb2_grpc
from transport.grpc.services.face_recognition_service import FaceRecognitionService
from data_access.qdrant_repository import QdrantRepository
from dotenv import load_dotenv
import os

logger = setup_logging();

if __name__ == "__main__":

    load_dotenv()

    server = grpc.server(
        futures.ThreadPoolExecutor(max_workers=10),
        interceptors=[
            CorrelationIdInterceptor(),
            EndRequestInterceptor()
        ]
    )

    vector_store = QdrantRepository()
    recognition_service = RecognitionService(vector_store=vector_store)
    
    face_recognition_pb2_grpc.add_FaceRecognitionServicer_to_server(
        FaceRecognitionService(recognition_service=recognition_service), server
    )




    grpc_port = os.getenv("GRPC_PORT")
    server.add_insecure_port(f'[::]:{grpc_port}')
    server.start()
    cid = set_correlation_id()

    try:
        logger.info(f"Server has started on port {grpc_port}.")
        while True:
            time.sleep(86400)
    except KeyboardInterrupt:
        logger.info("Server has stopped.")
        server.stop(0)
