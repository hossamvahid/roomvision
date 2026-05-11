import cv2
import requests
import os
import time
import logging
from dotenv import load_dotenv

load_dotenv()

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
)
log = logging.getLogger(__name__)

BACKEND_URL = os.getenv("BACKEND_URL")
ROOM_NAME = os.getenv("ROOM_NAME")
ROOM_PASSWORD = os.getenv("ROOM_PASSWORD")
CAMERA_INDEX = int(os.getenv("CAMERA_INDEX"))


DEBOUNCE_SECONDS = float(os.getenv("DEBOUNCE_SECONDS", "2"))

POLL_INTERVAL = float(os.getenv("POLL_INTERVAL", "1"))

STABILITY_FRAMES = int(os.getenv("STABILITY_FRAMES", "3"))


def authenticate(session: requests.Session) -> str:
    url = f"{BACKEND_URL}/api/v1/authentication/room"
    resp = session.post(url, json={"roomName": ROOM_NAME, "password": ROOM_PASSWORD}, timeout=10)
    resp.raise_for_status()
    token = resp.json()["token"]
    log.info("Authenticated as room '%s'", ROOM_NAME)
    return token


def send_scan(session: requests.Session, token: str, frame) -> bool:
    ok, buf = cv2.imencode(".jpg", frame)
    if not ok:
        log.error("Failed to encode frame as JPEG")
        return False

    url = f"{BACKEND_URL}/api/v1/room/scan"
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/octet-stream",
    }
    try:
        resp = session.post(
            url,
            params={"room": ROOM_NAME},
            data=buf.tobytes(),
            headers=headers,
            timeout=15,
        )
        if resp.status_code == 401:
            log.warning("Token expired, will re-authenticate")
            return False
        resp.raise_for_status()
        log.info("Scan sent successfully (status %d)", resp.status_code)
        return True
    except requests.RequestException as exc:
        log.error("Failed to send scan: %s", exc)
        return False


def count_faces(detector, frame) -> int:
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
    faces = detector.detectMultiScale(
        gray,
        scaleFactor=1.1,
        minNeighbors=5,
        minSize=(60, 60),
    )
    return len(faces) if len(faces) > 0 else 0


def main():
    if not ROOM_NAME or not ROOM_PASSWORD:
        raise SystemExit("ROOM_NAME and ROOM_PASSWORD must be set in .env")

    cascade_path = cv2.data.haarcascades + "haarcascade_frontalface_default.xml"
    detector = cv2.CascadeClassifier(cascade_path)
    if detector.empty():
        raise SystemExit(f"Failed to load Haar cascade from {cascade_path}")

    cap = cv2.VideoCapture(CAMERA_INDEX)
    if not cap.isOpened():
        raise SystemExit(f"Cannot open camera index {CAMERA_INDEX}")
    log.info("Camera opened (index %d)", CAMERA_INDEX)

    session = requests.Session()
    token = authenticate(session)

    last_confirmed_count = -1
    candidate_count = -1
    candidate_streak = 0
    last_send_time = 0.0

    try:
        while True:
            ret, frame = cap.read()
            if not ret:
                log.warning("Failed to grab frame, retrying…")
                time.sleep(1)
                continue

            current_count = count_faces(detector, frame)

           
            if current_count == candidate_count:
                candidate_streak += 1
            else:
                candidate_count = current_count
                candidate_streak = 1

            change_is_stable = (
                candidate_streak >= STABILITY_FRAMES
                and candidate_count != last_confirmed_count
            )

            if change_is_stable:
                now = time.monotonic()
                if now - last_send_time >= DEBOUNCE_SECONDS:
                    direction = (
                        "entered" if candidate_count > last_confirmed_count else "left"
                    )
                    log.info(
                        "Face count changed %d → %d (someone %s), sending scan…",
                        last_confirmed_count,
                        candidate_count,
                        direction,
                    )
                    success = send_scan(session, token, frame)
                    if success:
                        last_confirmed_count = candidate_count
                        last_send_time = now
                    else:
                        try:
                            token = authenticate(session)
                            success = send_scan(session, token, frame)
                            if success:
                                last_confirmed_count = candidate_count
                                last_send_time = now
                        except Exception as exc:
                            log.error("Re-auth failed: %s", exc)

            time.sleep(POLL_INTERVAL)

    except KeyboardInterrupt:
        log.info("Shutting down")
    finally:
        cap.release()


if __name__ == "__main__":
    main()
