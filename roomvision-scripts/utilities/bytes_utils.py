from PIL import Image
import io
import numpy as np

def bytes_to_numpy(image_bytes):
    """Convert image bytes to numpy array in RGB format"""
    pil_image = Image.open(io.BytesIO(image_bytes))
    return np.array(pil_image.convert('RGB'))