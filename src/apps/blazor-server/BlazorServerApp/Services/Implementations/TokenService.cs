using BlazorServerApp.Dtos;
using BlazorServerApp.Helpers;
using BlazorServerApp.HttpClients;
using BlazorServerApp.Services.Contracts;

namespace BlazorServerApp.Services.Implementations;

internal sealed class TokenService(CustomHttpClient httpClient) : ITokenService
{
    public const string baseUrl = "/token/accesstoken";

    public async Task<TokenResponse> GetTokenWithRefreshToken(TokenRequest request, string ipAddress, CancellationToken? cancellationToken = null)
    {
        HttpClient httpclient = httpClient.GetPublicHttpClient();

        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"{Constants.TokenBaseUrl}/refresh", request);
        TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return result!;
    }

    public async Task<TokenResponse> LoginUser(LoginDto request, CancellationToken? cancellationToken = null)
    {
        HttpClient httpclient = httpClient.GetPublicHttpClient();
        HttpResponseMessage response = await httpclient.PostAsJsonAsync($"{Constants.TokenBaseUrl}/accesstoken", request);

        if (response.IsSuccessStatusCode)
        {
            TokenResponse? result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return result!;
        }
        else
        {
            ApiErrorResponse? errorMessage = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            return new TokenResponse { Error = errorMessage?.Detail ?? "Unknown error occurred." };
        }

    }
}