using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Location404.Auth.Infrastructure.Services;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ocorreu um erro não tratado: {ErrorMessage}", exception.Message);
        ProblemDetails problemDetails = new()
        {
            Status = httpContext.Response.StatusCode,
            Detail = exception.Message,
            Instance = httpContext.Response.Headers.Referer,
        };

        problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = httpContext.Response.StatusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}