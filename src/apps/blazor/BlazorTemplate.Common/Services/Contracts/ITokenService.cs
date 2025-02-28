using BlazorTemplate.Common.Models;
using LoginDto = BlazorTemplate.Common.Models.LoginDto;
using TokenResponse = BlazorTemplate.Common.Models.TokenResponse;

namespace BlazorTemplate.Common.Services.Contracts;

public interface ITokenService
{
    Task<TokenResponse> LoginUser(LoginDto request, CancellationToken? cancellationToken = null);
    Task<TokenResponse> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken = null);
}
