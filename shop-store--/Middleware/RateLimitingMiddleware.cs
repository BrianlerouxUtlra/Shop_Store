using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Shop_store.Config;
using Shop_store.Services;
using System;
using System.Threading.Tasks;

namespace Shop_store.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _limit;
        private readonly TimeSpan _resetInterval;

        public RateLimitingMiddleware(RequestDelegate next, IOptions<RateLimitingOptions> options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _limit = options.Value.Limit;
            _resetInterval = TimeSpan.FromMinutes(options.Value.ResetIntervalMinutes);
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress.ToString();
            var requestCount = 1;

            if (requestCount > _limit)
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }

            await _next(context);
        }
    }
}