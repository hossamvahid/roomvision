FROM python:3.11-slim

RUN apt-get update && apt-get install -y \
    libgl1 \
    cmake \
    build-essential \
    libboost-all-dev \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
RUN pip install --upgrade pip
COPY requirements.txt /app/
RUN pip install --no-cache-dir -r requirements.txt

COPY protos /app/protos

RUN python -m grpc_tools.protoc \
    -I. \
    --python_out=. \
    --grpc_python_out=. \
    protos/*.proto
COPY . /app

EXPOSE 50051

CMD ["python", "main.py"]
