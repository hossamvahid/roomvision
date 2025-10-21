using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace roomvision.presentation.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = Guid.NewGuid().ToString();

            LogicalThreadContext.Properties["CorrelationId"] = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Append("X-Correlation-Id", correlationId);
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}