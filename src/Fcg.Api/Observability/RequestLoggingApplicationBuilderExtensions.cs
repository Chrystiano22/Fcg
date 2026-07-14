namespace Fcg.Api.Observability;

public static class RequestLoggingApplicationBuilderExtensions
{
    public static IApplicationBuilder UseStructuredRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
