/*
    File: AuthenticationMiddleware.cs
    Summary: Middleware for validating token-based authentication on all API endpoints except Swagger and root paths. 
    Checks for a hardcoded bearer token and logs authentication events.
*/namespace TechHiveAPI.Middleware;

/// <summary>
/// Middleware for token-based authentication.
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;
    private const string ValidToken = "TechHive2024SecureToken";

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Bypass authentication for Swagger and root paths
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.Value == "/" ||
            context.Request.Path.Value == "/index.html")
        {
            await _next(context);
            return;
        }

        // Check for Authorization header
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            _logger.LogWarning("Missing Authorization header for request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Missing authentication token.");
            return;
        }

        // Validate token format (Bearer <token>)
        var token = authHeader.ToString();
        if (!token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Invalid Authorization header format for request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid token format.");
            return;
        }

        // Extract and validate token
        var extractedToken = token.Substring("Bearer ".Length).Trim();
        if (extractedToken != ValidToken)
        {
            _logger.LogWarning("Invalid authentication token for request to {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid authentication token.");
            return;
        }

        // Token is valid, proceed to next middleware
        await _next(context);
    }
}
