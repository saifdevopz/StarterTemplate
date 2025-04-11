using System.ComponentModel.DataAnnotations;

namespace BlazorServerApp.Dtos;

internal sealed class LoginDto
{
    [Required]
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

internal sealed class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? Error { get; set; }
}

internal sealed record TokenRequest(string Token, string RefreshToken);

internal sealed record TokenClaimsResponse
(
    int? UserId = null,
    int? TenantId = null,
    string? Email = null,
    HashSet<string>? RoleName = null,
    string? TenantTypeCode = null,
    string? TenantName = null,
    string? DatabaseName = null,
    string? Expiry = null
);