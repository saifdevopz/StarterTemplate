using System.Security.Cryptography;
using System.Text;

namespace System.Domain.Identity;

public sealed class UserM : AggregateRoot
{
    public int UserId { get; }
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiration { get; set; }

    public static UserM Create(
        string email,
        string password)
    {
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

        UserM user = new()
        {
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        return user;
    }

    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using HMACSHA512 hmac = new();
        passwordSalt = hmac.Key;
        passwordHash = hmac
                .ComputeHash(Encoding.UTF8.GetBytes(password));
    }

}

