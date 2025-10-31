using System.Diagnostics;
using Shared.Observability.Core;

namespace Location404.Auth.API.Middleware;

public class ObservabilityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ActivitySource _activitySource;
    private readonly ObservabilityMetrics _metrics;
    private readonly ILogger<ObservabilityMiddleware> _logger;

    public ObservabilityMiddleware(
        RequestDelegate next,
        ActivitySource activitySource,
        ObservabilityMetrics metrics,
        ILogger<ObservabilityMiddleware> logger)
    {
        _next = next;
        _activitySource = activitySource;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "/";
        var method = context.Request.Method;
        var stopwatch = Stopwatch.StartNew();

        using var activity = _activitySource.StartActivity($"{method} {path}", ActivityKind.Server);

        activity?.SetTag("http.method", method);
        activity?.SetTag("http.path", path);
        activity?.SetTag("http.host", context.Request.Host.ToString());

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            activity?.SetTag("user.id", userId);
        }

        try
        {
            await _next(context);

            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            activity?.SetTag("http.status_code", statusCode);
            activity?.SetTag("http.response_time_ms", stopwatch.ElapsedMilliseconds);

            _metrics.IncrementRequests(method, path, statusCode);
            _metrics.RecordRequestDuration(stopwatch.Elapsed.TotalSeconds, method, path);

            if (statusCode >= 400)
            {
                var errorType = statusCode switch
                {
                    401 => "Unauthorized",
                    403 => "Forbidden",
                    404 => "NotFound",
                    400 => "BadRequest",
                    >= 500 => "InternalServerError",
                    _ => "ClientError"
                };

                _metrics.IncrementErrors(errorType, path);
                activity?.SetStatus(ActivityStatusCode.Error, $"HTTP {statusCode}");
            }
            else
            {
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("exception.stacktrace", ex.StackTrace);

            _metrics.IncrementErrors(ex.GetType().Name, path);
            _logger.LogError(ex, "Unhandled exception in {Path}", path);

            throw;
        }
    }
}
