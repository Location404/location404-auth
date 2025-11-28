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

        var statusCode = httpContext.Response.StatusCode == 200 ? StatusCodes.Status500InternalServerError : httpContext.Response.StatusCode;

        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Detail = exception.Message,
            Instance = httpContext.Response.Headers.Referer,
        };

        problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}