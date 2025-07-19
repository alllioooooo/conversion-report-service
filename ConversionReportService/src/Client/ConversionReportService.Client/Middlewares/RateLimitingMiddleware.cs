using ConversionReportService.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ConversionReportService.Client.Middlewares;

public class RateLimitingMiddleware : IMiddleware
{
    private readonly IRateLimiter _limiter;

    public RateLimitingMiddleware(IRateLimiter limiter)
    {
        _limiter = limiter;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var clientId = context.Request.Headers["X-R256-USER-IP"].FirstOrDefault()
                       ?? context.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrWhiteSpace(clientId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Unable to determine client identity for rate limiting");
            return;
        }

        if (!await _limiter.AllowAsync(clientId, context.RequestAborted))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }

        await next(context);
    }
}