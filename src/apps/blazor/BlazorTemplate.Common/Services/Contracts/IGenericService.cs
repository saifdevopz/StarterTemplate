namespace BlazorTemplate.Common.Services.Contracts;

public interface IGenericService<TRead, TWrite>
{
    Task<List<TRead>> GetAll(string basePath);
    Task<TRead> GetById(string basePath, int id);
    Task<HttpResponseMessage> Insert(string basePath, TWrite item);
    Task<bool> Update(string basePath, TWrite item);
    Task<bool> DeleteById(Uri basePath, int id);
}
