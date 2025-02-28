using Common.Domain.TransferObjects.System;

namespace System.Application.Common.Interfaces;

public interface ITokenService
{
    Task<Result<TokenResponse>> AccessToken(AccessTokenRequest request);
    Task<Result<TokenResponse>> RefreshToken(RefreshTokenRequest request);
    Task<TokenClaimsResponse> GetAllUserDetails(string email);
}
