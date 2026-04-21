import grpc  
from transport.grpc.protos import face_recognition_pb2_grpc
from transport.grpc.protos import face_recognition_pb2
from services.recognition_service import RecognitionService

class FaceRecognitionService(face_recognition_pb2_grpc.FaceRecognitionServicer):

    def __init__(self, recognition_service : RecognitionService):
        self.recognition_service = recognition_service

    def EmbedFace(self, request, context):
        
        return face_recognition_pb2.EmbedResponse(
            embedding=self.recognition_service.EmbedFace(
                    image_bytes=request.image,
                    person_name=request.person_name
                )
        )
   
    def VerifyRoom(self, request, context):
        matched_persons = self.recognition_service.VerifyRoom(image_bytes=request.image)
        return face_recognition_pb2.PersonsInRoom(persons=matched_persons, total=len(matched_persons))