using BlazorTemplate.Common.Delegates;
using BlazorTemplate.Common.Services.Contracts;
using System.Net.Http.Json;

namespace BlazorTemplate.Common.Services.Implementations;

public class GenericService<TRead, TWrite>(CustomHttpClient getHttpClient) : IGenericService<TRead, TWrite>
{
    public async Task<HttpResponseMessage> Insert(string basePath, TWrite item)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{basePath}/create", item);
        return response;
    }

    public async Task<List<TRead>> GetAll(string basePath)
    {
        HttpClient httpClient = getHttpClient.GetPublicHttpClient();
        List<TRead>? results = await httpClient.GetFromJsonAsync<List<TRead>>($"{basePath}/all");
        return results!;
    }

    public async Task<TRead> GetById(string basePath, int id)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        TRead? result = await httpClient.GetFromJsonAsync<TRead>($"{basePath}/single/{id}");
        return result!;
    }

    public async Task<bool> Update(string basePath, TWrite item)
    {
        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{basePath}/update", item);
        bool result = await response.Content.ReadFromJsonAsync<bool>();
        return result!;
    }

    public async Task<bool> DeleteById(Uri basePath, int id)
    {
        Uri deleteUri = new(basePath, $"/delete/{id}");

        HttpClient httpClient = await getHttpClient.GetPrivateHttpClient();
        HttpResponseMessage response = await httpClient.DeleteAsync(deleteUri);
        bool result = await response.Content.ReadFromJsonAsync<bool>();
        return result!;
    }

}