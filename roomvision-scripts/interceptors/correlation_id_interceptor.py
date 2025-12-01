import grpc
from logs.logging_config import get_logger, set_correlation_id

class CorrelationIdInterceptor(grpc.ServerInterceptor):

    def intercept_service(self, continuation, handler_call_details):
        logger = get_logger()
        correlation_id = set_correlation_id()

        logger.info(f"Incoming request: {handler_call_details.method}")

        handler = continuation(handler_call_details)

        def new_behavior(request, context):
            set_correlation_id(correlation_id)
            context.set_trailing_metadata((
                ('correlation-id', correlation_id),
            ))

            return handler.unary_unary(request, context)

        return grpc.unary_unary_rpc_method_handler(
            new_behavior,
            request_deserializer=handler.request_deserializer,
            response_serializer=handler.response_serializer
        )
