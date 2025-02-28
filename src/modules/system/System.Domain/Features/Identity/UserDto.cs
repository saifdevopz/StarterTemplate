namespace System.Domain.Features.Identity;

public sealed record CreateUserDto
(
    int TenantId,
    string Email,
    string Password
);

public class GetUserByIdDto
{
    public int UserId { get; set; }
    public int TenantId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
}
