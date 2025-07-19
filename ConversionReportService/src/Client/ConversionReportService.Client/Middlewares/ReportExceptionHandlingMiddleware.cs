using ConversionReportService.Application.Contracts.Exceptions;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConversionReportService.Client.Middlewares;

public class ExceptionFormattingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionFormattingMiddleware> _logger;

    public ExceptionFormattingMiddleware(ILogger<ExceptionFormattingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException rpcEx)
        {
            var statusCode = rpcEx.StatusCode switch
            {
                StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                StatusCode.NotFound => StatusCodes.Status404NotFound,
                StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
                StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
                StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
                StatusCode.Internal => StatusCodes.Status500InternalServerError,
                StatusCode.Cancelled => StatusCodes.Status499ClientClosedRequest,
                StatusCode.Unknown => StatusCodes.Status500InternalServerError,
                StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
                StatusCode.ResourceExhausted => StatusCodes.Status429TooManyRequests,
                StatusCode.FailedPrecondition => StatusCodes.Status400BadRequest,
                StatusCode.Aborted => StatusCodes.Status409Conflict,
                StatusCode.OutOfRange => StatusCodes.Status416RangeNotSatisfiable,
                StatusCode.Unimplemented => StatusCodes.Status501NotImplemented,
                StatusCode.DataLoss => StatusCodes.Status500InternalServerError,
                StatusCode.OK => StatusCodes.Status200OK,
                _ => StatusCodes.Status500InternalServerError,
            };

            _logger.LogWarning(rpcEx, "gRPC exception caught: {StatusCode} - {Message}", rpcEx.StatusCode, rpcEx.Status.Detail);

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                status = statusCode,
                message = rpcEx.Status.Detail
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                status = StatusCodes.Status500InternalServerError,
                error = ex.GetType().Name,
                message = ex.Message
            });
        }
    }
}