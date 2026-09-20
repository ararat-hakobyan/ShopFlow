using Microsoft.AspNetCore.Diagnostics;
using ShopFlow.Common;

namespace ShopFlow.Web;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            traceId);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse.Fail(
                "The request could not be completed. The problem has been written to the application log. " +
                $"Reference: {traceId}"),
            cancellationToken);

        // true means "this failure is dealt with", so nothing else tries to answer.
        return true;
    }
}
