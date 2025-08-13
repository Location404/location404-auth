using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UserIdentityService.Infrastructure.Services;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger = logger;

    /// <summary>
    /// Handles an exception by logging it and writing a ProblemDetails JSON payload to the HTTP response.
    /// </summary>
    /// <param name="httpContext">The current HTTP context whose response will be populated with the problem details.</param>
    /// <param name="exception">The exception to describe in the ProblemDetails payload.</param>
    /// <param name="cancellationToken">Cancellation token forwarded to the asynchronous response write operation.</param>
    /// <returns>True if the exception was handled and a response was written.</returns>
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