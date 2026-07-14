using Fcg.Application.Common;
using Fcg.Application.Security;
using Fcg.Api.Observability;
using Fcg.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.ErrorHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            DomainValidationException => (
                StatusCodes.Status400BadRequest,
                "Validation error",
                exception.Message),
            ResourceNotFoundException => (
                StatusCodes.Status404NotFound,
                "Resource not found",
                exception.Message),
            AuthenticationFailedException => (
                StatusCodes.Status401Unauthorized,
                "Authentication failed",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.")
        };
        var logLevel = exception switch
        {
            DomainValidationException => LogLevel.Warning,
            ResourceNotFoundException => LogLevel.Warning,
            AuthenticationFailedException => LogLevel.Warning,
            _ => LogLevel.Error
        };

        _logger.Log(
            logLevel,
            exception,
            "Handled exception {ExceptionType} for {RequestMethod} {RequestPath} with status code {StatusCode}",
            exception.GetType().Name,
            httpContext.Request.Method,
            httpContext.Request.Path.Value,
            statusCode);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.Headers[RequestLoggingMiddleware.CorrelationIdHeaderName] = httpContext.TraceIdentifier;

        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Extensions =
                {
                    ["correlationId"] = httpContext.TraceIdentifier
                }
            }
        });

        return true;
    }
}
