FROM python:3.11

# System dependencies for OpenCV + dlib
RUN apt-get update && apt-get install -y \
    libgl1 \
    cmake \
    build-essential \
    libboost-all-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app



# Upgrade pip first (important)
RUN pip install --upgrade pip

COPY requirements.txt /app/

# Install Python dependencies
RUN pip install --no-cache-dir -r requirements.txt

COPY protos /app/protos

# Generate gRPC Python code inside container (version-safe)
RUN python -m grpc_tools.protoc \
    -I. \
    --python_out=. \
    --grpc_python_out=. \
    protos/*.proto



COPY . /app

EXPOSE 50051

CMD ["python", "main.py"]
