import face_recognition
from logs.logging_config import get_logger
import numpy as np
import cv2
from mtcnn import MTCNN

detector = MTCNN()

def embed_face(image, known_face_locations=None):
    """Encode a face from a numpy image array."""
    encodings = face_recognition.face_encodings(face_image=image, known_face_locations=known_face_locations,model="medium")

    if len(encodings) == 0:
        return None

    return encodings[0]

def detect_faces(image):
    """Detect faces from an image."""
    result = detector.detect_faces(image)

    face_locations = []
    for face in result:
        x, y, width, height = face['box']
        top = y
        right = x + width
        bottom = y + height
        left = x
        face_locations.append((top, right, bottom, left))

    return face_locations


def compare_faces(known_encoding, unknown_encoding, tolerance=0.6):
    """Compare an known face encoding against an unknown face encoding."""
    return face_recognition.compare_faces([known_encoding], unknown_encoding, tolerance=tolerance)[0]


def find_faces(image, unknown_faces_location, vector_store, tolerance=0.6):
    """Find matching faces in the vector store for a list of unknown face encodings."""
    logger = get_logger()

    logger.info("Starting to find matching faces in the vector store")
    matched_faces = []

    
    for unknown_face_location in unknown_faces_location:

        logger.info("Encoding unknown face")
        unknown_encoding = embed_face(image=image, known_face_locations=[unknown_face_location])

        logger.info("Searching vector database for matching persons")
        face_found = vector_store.search(vector=unknown_encoding, limit=1)

        if face_found is None:
            logger.info("No matching person found for this face")
            matched_faces.append("Unknown")
            continue

        logger.info(f"Found potential match: {face_found.getPersonName()} with ID: {face_found.getId()}")

        if compare_faces(known_encoding=face_found.getEmbedding(), unknown_encoding=unknown_encoding, tolerance=tolerance) == False:
            logger.info("No matching person found for this face")
            matched_faces.append("Unknown")
            continue
        
        logger.info(f"Person {face_found.getPersonName()} verified")
        matched_faces.append(face_found.getPersonName())

    return matched_faces