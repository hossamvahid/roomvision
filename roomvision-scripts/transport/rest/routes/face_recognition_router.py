from fastapi import APIRouter, File, Form, UploadFile, HTTPException
from pydantic import BaseModel
from services.recognition_service import RecognitionService
from logs.logging_config import get_logger


class EmbedResponse(BaseModel):
    embedding: list[float]


class VerifyRoomResponse(BaseModel):
    persons: list[str]
    total: int


def create_router(recognition_service: RecognitionService) -> APIRouter:
    router = APIRouter()

    @router.post("/embed-face", response_model=EmbedResponse)
    async def embed_face(
        image: UploadFile = File(...),
        person_name: str = Form(...),
    ):
        logger = get_logger()
        logger.info("Received EmbedFace request")

        image_bytes = await image.read()
        embedding = recognition_service.EmbedFace(
            image_bytes=image_bytes,
            person_name=person_name,
        )

        if embedding is None:
            raise HTTPException(status_code=404, detail="No face detected in the image")

        return EmbedResponse(embedding=embedding)

    @router.post("/verify-room", response_model=VerifyRoomResponse)
    async def verify_room(image: UploadFile = File(...)):
        logger = get_logger()
        logger.info("Received VerifyRoom request")

        image_bytes = await image.read()
        persons = recognition_service.VerifyRoom(image_bytes=image_bytes)

        return VerifyRoomResponse(persons=persons, total=len(persons))

    return router
