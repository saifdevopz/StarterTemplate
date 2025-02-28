using BlazorTemplate.Common.Helpers;
using BlazorTemplate.Common.Models;
using BlazorTemplate.Common.Services.Contracts;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorTemplate.Common.Delegates;

public sealed class TokenDelegate(LocalStorageService localStorageService, ITokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        bool loginUrl = request.RequestUri!.AbsoluteUri.Contains("login", StringComparison.OrdinalIgnoreCase);
        bool registerUrl = request.RequestUri!.AbsoluteUri.Contains("accesstoken", StringComparison.OrdinalIgnoreCase);
        bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains("refresh-token", StringComparison.OrdinalIgnoreCase);

        if (loginUrl || registerUrl || refreshTokenUrl)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        HttpResponseMessage result = await base.SendAsync(request, cancellationToken);
        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            string? stringToken = await localStorageService.GetToken();
            if (stringToken == null)
            {
                return result;
            }

            string token = string.Empty;
            try
            {
                token = request.Headers.Authorization!.Parameter!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            TokenResponse? deserializedToken = Serialization.DeserializeJsonString<TokenResponse>(stringToken);
            if (deserializedToken is null)
            {
                return result;
            }

            if (string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                return await base.SendAsync(request, cancellationToken);
            }

            // Call for refresh token.
            TokenResponse newJwtToken = await GetRefreshToken(deserializedToken, cancellationToken);
            if (string.IsNullOrEmpty(newJwtToken.Token))
            {
                return result;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newJwtToken.Token);

            return await base.SendAsync(request, cancellationToken);
        }

        return result;
    }

    private async Task<TokenResponse> GetRefreshToken(TokenResponse tokens, CancellationToken cancellationToken)
    {
        TokenResponse result = await tokenService.GetTokenWithRefreshToken(new TokenRequest(tokens.Token, tokens.RefreshToken), "N/A", cancellationToken);
        string serializedToken = Serialization.SerializeObj(new TokenResponse() { Token = result.Token, RefreshToken = result.RefreshToken, RefreshTokenExpiryTime = result.RefreshTokenExpiryTime });
        await localStorageService.SetToken(serializedToken);
        return result!;
    }

}