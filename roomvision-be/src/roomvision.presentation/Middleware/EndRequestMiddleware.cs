using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using log4net;

namespace roomvision.presentation.Middleware
{
    public class EndRequestMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILog log;

        public EndRequestMiddleware(RequestDelegate next, ILog log)
        {
            this.next = next;
            this.log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                log.Error($"Request failed error: {e.ToString()}");
                context.Response.Clear();

                var errorMessage = new { message = "Something went wrong with server" };
                var jsonErrorMessage = JsonSerializer.Serialize(errorMessage.message);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(jsonErrorMessage);
                context.Response.StatusCode = 500;
            }
            finally
            {
                stopwatch.Stop();
                log.Info($"Response: Status Code: {context.Response?.StatusCode}, Latency: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}