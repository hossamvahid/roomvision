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


# Minimum gap between two scans sent to the backend (anti-hammer).
DEBOUNCE_SECONDS = float(os.getenv("DEBOUNCE_SECONDS", "1"))

# Pause between processed frames. Small value = the loop reacts within a few
# frames instead of once per second. Detection on the Pi is cheap enough that
# this mainly caps CPU usage / fps.
POLL_INTERVAL = float(os.getenv("POLL_INTERVAL", "0.05"))

# How many consecutive frames must agree on the new face count before we accept
# the change. Fewer frames = faster reaction, more = fewer false triggers.
STABILITY_FRAMES = int(os.getenv("STABILITY_FRAMES", "2"))

# --- Pi performance tuning (all overridable via .env) ---
# Resolution the camera captures at. Lower = far less CPU/bandwidth on a Pi.
CAPTURE_WIDTH = int(os.getenv("CAPTURE_WIDTH", "640"))
CAPTURE_HEIGHT = int(os.getenv("CAPTURE_HEIGHT", "480"))
# Width the frame is downscaled to *only* for face detection. The full-res
# frame is still what gets sent to the backend for recognition.
DETECTION_WIDTH = int(os.getenv("DETECTION_WIDTH", "320"))
# JPEG quality for the frame uploaded to the backend (1-100).
JPEG_QUALITY = int(os.getenv("JPEG_QUALITY", "80"))


def authenticate(session: requests.Session) -> str:
    url = f"{BACKEND_URL}/api/v1/authentication/room"
    resp = session.post(url, json={"roomName": ROOM_NAME, "password": ROOM_PASSWORD}, timeout=10)
    resp.raise_for_status()
    token = resp.json()["token"]
    log.info("Authenticated as room '%s'", ROOM_NAME)
    return token


def send_scan(session: requests.Session, token: str, frame) -> bool:
    ok, buf = cv2.imencode(
        ".jpg", frame, [int(cv2.IMWRITE_JPEG_QUALITY), JPEG_QUALITY]
    )
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

    # Downscale for detection: Haar cascades are O(pixels), so running on a
    # ~320px-wide image instead of the full frame is several times faster on a
    # Raspberry Pi while still detecting people at room distance.
    h, w = gray.shape[:2]
    if DETECTION_WIDTH and w > DETECTION_WIDTH:
        scale = DETECTION_WIDTH / w
        gray = cv2.resize(
            gray,
            (DETECTION_WIDTH, int(h * scale)),
            interpolation=cv2.INTER_AREA,
        )

    # Normalise lighting so detection is more stable under changing room light.
    gray = cv2.equalizeHist(gray)

    faces = detector.detectMultiScale(
        gray,
        scaleFactor=1.1,
        minNeighbors=5,
        minSize=(40, 40),
    )
    return len(faces)


def open_camera() -> cv2.VideoCapture:
    cap = cv2.VideoCapture(CAMERA_INDEX)
    if not cap.isOpened():
        raise SystemExit(f"Cannot open camera index {CAMERA_INDEX}")

    # MJPG lets most USB cameras deliver frames at higher fps with less CPU on
    # the Pi (the sensor does the JPEG work instead of the CPU).
    cap.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc(*"MJPG"))
    cap.set(cv2.CAP_PROP_FRAME_WIDTH, CAPTURE_WIDTH)
    cap.set(cv2.CAP_PROP_FRAME_HEIGHT, CAPTURE_HEIGHT)
    # Keep only the newest frame in the driver buffer so we never process a
    # stale image queued while we were sleeping/uploading.
    cap.set(cv2.CAP_PROP_BUFFERSIZE, 1)

    # Warm up: discard the first few frames while the sensor auto-exposes.
    for _ in range(5):
        cap.read()

    log.info(
        "Camera opened (index %d) at %dx%d",
        CAMERA_INDEX,
        int(cap.get(cv2.CAP_PROP_FRAME_WIDTH)),
        int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT)),
    )
    return cap


def main():
    if not ROOM_NAME or not ROOM_PASSWORD:
        raise SystemExit("ROOM_NAME and ROOM_PASSWORD must be set in .env")

    cascade_path = cv2.data.haarcascades + "haarcascade_frontalface_default.xml"
    detector = cv2.CascadeClassifier(cascade_path)
    if detector.empty():
        raise SystemExit(f"Failed to load Haar cascade from {cascade_path}")

    cap = open_camera()

    session = requests.Session()
    token = authenticate(session)

    last_confirmed_count = -1
    candidate_count = -1
    candidate_streak = 0
    last_send_time = 0.0

    try:
        while True:
            cap.grab()
            ret, frame = cap.retrieve()
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
