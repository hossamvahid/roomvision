import cv2
import numpy as np

def bytes_to_image(image_bytes):
    nparr = np.frombuffer(image_bytes, np.uint8)
    image_bgr = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    return image_bgr