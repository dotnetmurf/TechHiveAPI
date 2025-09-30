using System.Diagnostics;

namespace TechHiveAPI.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses.
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestTime = DateTime.UtcNow;

        _logger.LogInformation(
            "Incoming Request: {Method} {Path} at {Time}",
            context.Request.Method,
            context.Request.Path,
            requestTime);

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "Outgoing Response: {Method} {Path} responded {StatusCode} in {Duration}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}
