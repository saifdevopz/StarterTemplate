namespace Child1.API.Middleware;

internal static class MiddlewareExtensions
{
    internal static IApplicationBuilder UseLogContextTraceLogging(this IApplicationBuilder app)
    {
        //app.UseMiddleware<RestrictAccessMiddleware>(); // a trigger
        app.UseMiddleware<LogContextTraceLoggingMiddleware>();

        return app;
    }
}
