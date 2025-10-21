using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

namespace roomvision.presentation.Middleware
{
    public class StartRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILog _log;
        public StartRequestMiddleware(RequestDelegate next, ILog log)
        {
            _next = next;
            _log = log;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var fowardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            var ipAddress = fowardedFor ?? context.Connection.RemoteIpAddress?.ToString();

            _log.Info($"Incoming request from IP {ipAddress} for {context.Request.Protocol} , {context.Request.Method} , {context.Request.Path}");
            context.Response.Headers["Server"] = "RoomVisionServer";
            await _next(context);
        }
    }
}