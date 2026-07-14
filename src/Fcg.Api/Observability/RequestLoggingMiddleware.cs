using System.Diagnostics;
using System.Security.Claims;

namespace Fcg.Api.Observability;

public sealed class RequestLoggingMiddleware
{
    public const string CorrelationIdHeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var correlationId = ResolveCorrelationId(httpContext);
        var stopwatch = Stopwatch.StartNew();

        httpContext.TraceIdentifier = correlationId;
        httpContext.Response.Headers[CorrelationIdHeaderName] = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            _logger.LogDebug(
                "Handling HTTP request {RequestMethod} {RequestPath}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value);

            await _next(httpContext);

            stopwatch.Stop();

            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = httpContext.User.FindFirstValue(ClaimTypes.Role);
            var endpoint = httpContext.GetEndpoint()?.DisplayName;
            var logLevel = ResolveLogLevel(httpContext.Response.StatusCode);

            _logger.Log(
                logLevel,
                "Completed HTTP request {RequestMethod} {RequestPath} with status code {StatusCode} in {ElapsedMilliseconds}ms for endpoint {EndpointName}. Authenticated: {IsAuthenticated}. UserId: {UserId}. UserRole: {UserRole}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value,
                httpContext.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                endpoint,
                httpContext.User.Identity?.IsAuthenticated ?? false,
                userId,
                userRole);
        }
    }

    private static string ResolveCorrelationId(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var values))
        {
            var headerValue = values.ToString().Trim();
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                return headerValue;
            }
        }

        if (!string.IsNullOrWhiteSpace(httpContext.TraceIdentifier))
        {
            return httpContext.TraceIdentifier;
        }

        return Guid.NewGuid().ToString("N");
    }

    private static LogLevel ResolveLogLevel(int statusCode)
    {
        return statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            _ => LogLevel.Information
        };
    }
}
