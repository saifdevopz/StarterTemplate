using Serilog.Context;
using System.Diagnostics;

namespace Gateway.API.Middleware;

internal sealed class LogContextTraceMiddleware(RequestDelegate next)
{
    public Task Invoke(HttpContext context)
    {
        string traceId = Activity.Current?.TraceId.ToString()!;

        using (LogContext.PushProperty("TraceId", traceId))
        {
            return next.Invoke(context);
        }
    }
}