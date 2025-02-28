using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;

namespace Gateway.API.Middleware;

internal sealed class TokenValidatorMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Define paths that bypass token validation
        HashSet<string> allowedPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/",
            "/health",
            "/scalar/v1",
            "/openapi/v1.json",
            "/token/accesstoken",
            "/token/refreshtoken"
        };

        string requestPath = context.Request.Path.Value!;
        if (allowedPaths.Contains(requestPath))
        {
            await _next(context);
            return;
        }

        // Check if Authorization header exists
        if (!context.Request.Headers.TryGetValue("Authorization", out StringValues authHeader) || string.IsNullOrWhiteSpace(authHeader))
        {
            await RespondWithUnauthorized(context, "Authorization header is missing.");
            return;
        }

        // Extract the token
        string? token = authHeader.ToString().Split(" ").LastOrDefault();
        if (string.IsNullOrEmpty(token))
        {
            await RespondWithUnauthorized(context, "Token is missing.");
            return;
        }

        // Validate token expiration
        try
        {
            if (IsTokenExpired(token))
            {
                await RespondWithUnauthorized(context, "Token expired.");
                return;
            }
        }
        catch (ArgumentException ex)
        {
            await RespondWithUnauthorized(context, ex.Message);
            return;
        }

        // If token is valid, continue processing the request
        await _next(context);
    }
    public static bool IsTokenExpired(string token)
    {
        JwtSecurityTokenHandler handler = new();

        // Validate if the token is a valid JWT token
        if (!handler.CanReadToken(token))
        {
            throw new ArgumentException("Invalid JWT token");
        }

        // Read the token
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);

        // Extract the exp claim (expiration time)
        string exp = jwtToken.Claims.First(claim => claim.Type == "exp").Value;

        // Convert exp claim to DateTime
        DateTime expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;

        // Compare expiration time with the current time
        return expirationTime < DateTime.UtcNow;
    }

    private static async Task RespondWithUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync(message);
    }
}
