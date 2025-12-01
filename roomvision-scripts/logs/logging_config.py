import contextvars
import logging
import os
import uuid

correlation_id = contextvars.ContextVar('correlation_id', default='no-correlation-id')

class CorrelationIdFilter(logging.Filter):
    def filter(self, record):
        record.correlationId = correlation_id.get()
        return True


_logging_initialized = False
_logger_instance = None

def setup_logging():
    global _logging_initialized, _logger_instance
    
    if not _logging_initialized:
        os.makedirs('logs', exist_ok=True)
        
        correlation_filter = CorrelationIdFilter()
        
        file_handler = logging.FileHandler("logs/app.log", encoding='utf-8')
        file_handler.addFilter(correlation_filter)
        
        stream_handler = logging.StreamHandler()
        stream_handler.addFilter(correlation_filter)
        
        logging.basicConfig(
            level=logging.INFO,
            format='%(correlationId)s - %(levelname)s - %(message)s',
            handlers=[file_handler, stream_handler]
        )
        
        _logger_instance = logging.getLogger("RoomVision-FaceRecognition-Server")
        _logging_initialized = True

    return _logger_instance

def get_logger():
    if _logger_instance is None:
        setup_logging()
    return _logger_instance

def set_correlation_id(corr_id = None):
    cid = corr_id or str(uuid.uuid4())
    correlation_id.set(cid)
    return cid