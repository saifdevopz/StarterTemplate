namespace Gateway.API.Middleware;

internal sealed class AddReferrerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers["Referrer"] = "Api-Gateway";
        await next(context);
    }
}