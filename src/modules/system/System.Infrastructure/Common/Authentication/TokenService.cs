using Common.Application.Authentication;
using Common.Application.Database;
using Common.Domain.Errors;
using Common.Domain.Results;
using Common.Domain.TransferObjects.System;
using Common.Infrastructure.Authentication;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Application.Common.Interfaces;
using System.Domain.Features.Tenant;
using System.Domain.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Infrastructure.Common.Database;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace System.Infrastructure.Common.Authentication;

public class TokenService(
    IOptions<JwtOptions> JwtOptions,
    IRepository<UserM> Repository,
    IDbConnectionFactory Connection,
    SystemDbContext SystemContext) : ITokenService
{
    public async Task<Result<TokenResponse>> AccessToken(AccessTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        UserM userDto = Repository.FindOne(p => p.Email == request.Email);

        if (userDto is null)
        {
            return Result.Failure<TokenResponse>(CustomError.NotFound("TokenService", "User not found."));
        }
        else
        {
            if (!IdentityMethodExtensions.VerifyPasswordHash(request.Password, userDto.PasswordHash, userDto.PasswordSalt))
            {
                return Result.Failure<TokenResponse>(CustomError.Conflict("TokenService", "Invalid Credentials."));
            }
        }

        return await GenerateTokensAndUpdateUser(userDto);
    }

    public async Task<Result<TokenResponse>> RefreshToken(RefreshTokenRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ClaimsPrincipal userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        UserM user = Repository.FindOne(p => p.Email == userPrincipal.GetUserEmail());

        if (user is null)
        {
            return Result.Failure<TokenResponse>(CustomError.NotFound("TokenService", "User not found."));
        }

        TenantM? tenant = await SystemContext.Tenants.FirstOrDefaultAsync(t => t.TenantId == 1);
        ArgumentNullException.ThrowIfNull(tenant);

        return user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow
            ? Result.Failure<TokenResponse>(CustomError.NotFound("TokenService", "Invalid Refresh Token."))
            : await GenerateTokensAndUpdateUser(user);
    }

    private async Task<Result<TokenResponse>> GenerateTokensAndUpdateUser(UserM user)
    {
        TokenClaimsResponse tokenClaims = await GetAllUserDetails(user.Email);
        string token = GenerateJwt(tokenClaims);

        user.RefreshToken = IdentityMethodExtensions.GenerateRefreshToken();
        user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(JwtOptions.Value.RefreshTokenExpirationInDays);

        Repository.Update(user);
        await Repository.SaveChangesAsync();

        return new TokenResponse()
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiration
        };
    }

    private string GenerateJwt(TokenClaimsResponse customClaims)
    {
        return GenerateEncryptedToken(GetSigningCredentials(), GetClaims(customClaims));
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        JwtSecurityToken token = new(
             claims: claims,
             expires: DateTime.UtcNow.AddMinutes(JwtOptions.Value.TokenExpirationInMinutes),
             signingCredentials: signingCredentials,
             issuer: JwtOptions.Value.Issuer,
             audience: JwtOptions.Value.Audience
                                                                        );
        JwtSecurityTokenHandler tokenHandler = new();
        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials()
    {
        byte[] secret = Encoding.UTF8.GetBytes(JwtOptions.Value.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

    private static List<Claim> GetClaims(TokenClaimsResponse customClaims)
    {
        List<Claim> claims =
        [
            new Claim("UserId", customClaims.UserId.ToString()!),
            new Claim("TenantId", customClaims.TenantId.ToString()!),
            new Claim(ClaimTypes.Email, customClaims.Email!),
            new Claim("TenantTypeCode", customClaims.TenantTypeCode!),
            new Claim("TenantName", customClaims.TenantName!),
            new Claim(CustomClaims.DatabaseName, customClaims.DatabaseName!)
        ];

        if (customClaims.RoleName != null)
        {
            foreach (string role in customClaims.RoleName)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        return claims;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Value.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = JwtOptions.Value.Audience,
            ValidIssuer = JwtOptions.Value.Issuer,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken? securityToken);

        return securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase)
            ? throw new SecurityTokenValidationException("Invalid token.")
            : principal;
    }

    public async Task<TokenClaimsResponse> GetAllUserDetails(string email)
    {
        const string sql =
        $"""
            SELECT 
                u.UserId,
                t.TenantId,
                u.Email,
                r.RoleName,
                tt.TenantTypeCode,
                t.TenantName,
                t.DatabaseName
            FROM MAIN.Users u
            INNER JOIN MAIN.UserRoles ur ON ur.UserId = u.UserId
            INNER JOIN MAIN.Roles r ON r.RoleId = ur.RoleId
            INNER JOIN MAIN.TenantUsers tu ON tu.UserId = u.UserId
            INNER JOIN MAIN.Tenants t ON t.TenantId = tu.TenantId
            INNER JOIN MAIN.TenantTypes tt ON tt.TenantTypeId = t.TenantId
            WHERE u.Email = @email
        """;

        List<CustomUserClaim> obj = (await Connection.QueryAsync<CustomUserClaim>(sql, new { email }, true)).AsList();

        return new TokenClaimsResponse
            (obj[0].UserId,
            obj[0].TenantId,
            obj[0].Email,
            obj.Select(p => p.RoleName).ToHashSet(),
            obj[0].TenantTypeCode,
            obj[0].TenantName,
            obj[0].DatabaseName);
    }

    private static class IdentityMethodExtensions
    {
        public static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new(passwordSalt);
            byte[] computedHash =
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(passwordHash);
        }
    }
}


