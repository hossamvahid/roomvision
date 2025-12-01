import grpc
import time
from logs.logging_config import get_logger

class EndRequestInterceptor(grpc.ServerInterceptor):

    def intercept_service(self, continuation, handler_call_details):
        handler = continuation(handler_call_details)

        def new_behavior(request, context):
            logger = get_logger()
            start = time.time()

            try:
                response = handler.unary_unary(request, context)
                return response

            except Exception as e:
                logger.error(f"Request failed: {e}")
                context.set_code(grpc.StatusCode.INTERNAL)
                context.set_details("Something went wrong on the server.")
                raise

            finally:
                duration = (time.time() - start) * 1000
                logger.info(f"Request {handler_call_details.method} finished in {duration:.2f} ms")

        return grpc.unary_unary_rpc_method_handler(
            new_behavior,
            request_deserializer=handler.request_deserializer,
            response_serializer=handler.response_serializer
        )
