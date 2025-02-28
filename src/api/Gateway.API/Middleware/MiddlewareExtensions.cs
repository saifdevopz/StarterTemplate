namespace Gateway.API.Middleware;

internal static class MiddlewareExtensions
{
    internal static IApplicationBuilder UseLogContextTraceLogging(this IApplicationBuilder app)
    {
        //app.UseMiddleware<TokenValidatorMiddleware>();
        app.UseMiddleware<LogContextTraceMiddleware>();
        //app.UseMiddleware<TenantMiddleware>();

        return app;
    }
}