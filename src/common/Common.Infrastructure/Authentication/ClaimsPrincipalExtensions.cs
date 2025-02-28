using Common.Application.Exceptions;
using System.Security.Claims;

namespace Common.Infrastructure.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(userId, out int parsedUserId) ?
                parsedUserId :
                throw new StarterException("User identifier is unavailable");
    }

    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                     throw new StarterException("User identity is unavailable");
    }

    public static string GetUserEmail(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(ClaimTypes.Email)?.Value ??
                     throw new StarterException("User Email is unavailable");
    }

    public static string GetTenant(this ClaimsPrincipal? principal)
    {
        string? tenantClaim = principal?.FindFirst("Tenant")?.Value;
        return tenantClaim ?? string.Empty;
    }
    public static string GetTenantDbName(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirst(CustomClaims.DatabaseName)?.Value ?? string.Empty;
    }

    public static HashSet<string> GetPermissions(this ClaimsPrincipal? principal)
    {
        IEnumerable<Claim> permissionClaims = principal?.FindAll(CustomClaims.Permission) ??
            throw new StarterException("Permissions are unavailable");

        return permissionClaims.Select(c => c.Value).ToHashSet();
    }
}
