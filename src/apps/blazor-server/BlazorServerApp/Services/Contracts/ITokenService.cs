using BlazorServerApp.Dtos;

namespace BlazorServerApp.Services.Contracts;

internal interface ITokenService
{
    Task<TokenResponse> LoginUser(LoginDto request, CancellationToken? cancellationToken = null);
    Task<TokenResponse> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken = null);
}
