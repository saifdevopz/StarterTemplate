using BlazorTemplate.Common.Delegates;
using BlazorTemplate.Common.Helpers;
using BlazorTemplate.Common.Models;
using BlazorTemplate.Common.Services.Contracts;
using System.Net.Http.Json;

namespace BlazorTemplate.Common.Services.Implementations;

public class TokenService(CustomHttpClient httpClient) : ITokenService
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
            //ApiErrorResponse? errorMessage = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            return new TokenResponse();
        }

    }

    //internal sealed class ApiErrorResponse
    //{
    //    public string? Type { get; set; }
    //    public string? Title { get; set; }
    //    public int Status { get; set; }
    //    public string? Detail { get; set; }
    //    public string? TraceId { get; set; }
    //}

}