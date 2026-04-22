from utilities.face_recognition_utils import RecognitionUtil
from utilities import bytes_utils
from logs.logging_config import get_logger
from data_access.interface import VectorStore
from uuid import uuid4

class RecognitionService():
    def __init__(self, vector_store  : VectorStore, recognition_util : RecognitionUtil):
        self.vector_store = vector_store
        self.recognition_util = recognition_util


    def EmbedFace(self, image_bytes, person_name):
        logger = get_logger()
        logger.info("Received EmbedFace request")

        logger.info("Converting bytes to numpy array for face recognition")
        image = bytes_utils.bytes_to_image(image_bytes)

        logger.info("Embedding face")
        embedding = self.recognition_util.embed_face(image)

        if embedding is None:
            logger.warning("No face detected in the image")
            return None

        logger.info("Face embedded successfully")
        
        logger.info("Storing embedding in vector store")
        point_id = str(uuid4())

        payload = self.vector_store.create_payload(person_name=person_name)
        self.vector_store.add_vector(point_id=point_id, vector=embedding, payload=payload)
        
        logger.info("Embedding stored successfully")
        
        return list(embedding)
    
    def VerifyRoom(self, image_bytes):
        logger = get_logger()
        logger.info("Received VerifyRoom request")

        logger.info("Converting bytes to numpy array for face recognition")
        image = bytes_utils.bytes_to_image(image_bytes)

        logger.info("Detecting faces in the image")
        face_locations = self.recognition_util.detect_faces(image=image)

        if len(face_locations) == 0:
            logger.warning("No faces detected in the room")
            return []
        
        logger.info(f"Found {len(face_locations)} faces in the image")

        matched_persons = self.recognition_util.find_faces(
            image=image,
            unknown_faces_location=face_locations,
            vector_store=self.vector_store,
            tolerance=0.5
        )
        
        return matched_persons

    
