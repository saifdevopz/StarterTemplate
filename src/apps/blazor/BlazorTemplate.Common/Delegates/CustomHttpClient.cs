using BlazorTemplate.Common.Helpers;
using BlazorTemplate.Common.Models;
using System.Net.Http.Headers;

namespace BlazorTemplate.Common.Delegates;

public sealed class CustomHttpClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorageService)
{
    private const string HeaderKey = "Authorization";
    public async Task<HttpClient> GetPrivateHttpClient()
    {
        HttpClient client = httpClientFactory.CreateClient("SystemApiClient");
        string? stringToken = await localStorageService.GetToken();
        if (string.IsNullOrEmpty(stringToken))
        {
            return client;
        }

        TokenResponse deserializeToken = Serialization.DeserializeJsonString<TokenResponse>(stringToken);
        if (deserializeToken == null)
        {
            return client;
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", deserializeToken.Token);
        return client;
    }

    public HttpClient GetPublicHttpClient()
    {
        HttpClient client = httpClientFactory.CreateClient("SystemApiClient");
        client.DefaultRequestHeaders.Remove(HeaderKey);
        return client;
    }


}