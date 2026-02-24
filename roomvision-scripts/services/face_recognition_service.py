import grpc  
from protos import face_recognition_pb2, face_recognition_pb2_grpc
from utilities import bytes_utils
from logs.logging_config import get_logger
from data_access.interface import VectorStore
from uuid import uuid4
from utilities.face_recognition_utils import embed_face , detect_faces, find_faces

class FaceRecognitionService(face_recognition_pb2_grpc.FaceRecognitionServicer):

    def __init__(self, vector_store : VectorStore):
        self.vector_store = vector_store

    def EmbedFace(self, request, context):
        logger = get_logger()
        logger.info("Received EmbedFace request")

        image_bytes = request.image
        person_name = request.person_name

        logger.info("Converting bytes to numpy array for face recognition")
        image = bytes_utils.bytes_to_image(image_bytes)

        logger.info("Embedding face")
        embedding = embed_face(image)

        if embedding is None:
            logger.warning("No face detected in the image")
            context.set_code(grpc.StatusCode.NOT_FOUND)
            context.set_details("No face detected")
            return face_recognition_pb2.EncodeResponse()

        logger.info("Face embedded successfully")
        
        logger.info("Storing embedding in vector store")
        point_id = str(uuid4())

        payload = self.vector_store.create_payload(person_name=person_name)
        self.vector_store.add_vector(point_id=point_id, vector=embedding, payload=payload)
        
        logger.info("Embedding stored successfully")
        
        return face_recognition_pb2.EmbedResponse(
            embedding=list(embedding)
        )
   
    def VerifyRoom(self, request, context):
        logger = get_logger()
        logger.info("Received VerifyRoom request")

        image_bytes = request.image

        logger.info("Converting bytes to numpy array for face recognition")
        image = bytes_utils.bytes_to_image(image_bytes=image_bytes)

        logger.info("Detecting faces in the image")
        face_locations = detect_faces(image=image)

        if len(face_locations) == 0:
            logger.warning("No faces detected in the room")
            return face_recognition_pb2.PersonsInRoom(person_names=[])
        
        logger.info(f"Found {len(face_locations)} faces in the image")

        matched_persons = find_faces(
            image=image,
            unknown_faces_location=face_locations,
            vector_store=self.vector_store,
            tolerance=0.5
        )
        
        return face_recognition_pb2.PersonsInRoom(persons=matched_persons,total=len(matched_persons))