import time
import uuid
from starlette.middleware.base import BaseHTTPMiddleware
from logs.logging_config import get_logger, set_correlation_id


class CorrelationIdMiddleware(BaseHTTPMiddleware):

    async def dispatch(self, request, call_next):
        correlation_id = request.headers.get("X-Correlation-ID") or str(uuid.uuid4())
        set_correlation_id(correlation_id)

        logger = get_logger()
        logger.info(f"Incoming request: {request.method} {request.url.path}")

        start = time.time()
        response = await call_next(request)
        duration = (time.time() - start) * 1000

        logger.info(f"Request {request.method} {request.url.path} finished in {duration:.2f} ms")

        response.headers["X-Correlation-ID"] = correlation_id
        return response
