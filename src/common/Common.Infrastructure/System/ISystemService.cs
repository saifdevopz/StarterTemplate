
namespace Common.Infrastructure.System;
public interface ISystemService
{
    Task<List<T>> GetAllEntities<T>(string sqlQuery, CancellationToken? cancellationToken = null);
    Task<List<TenantDto>> GetParentTenants(CancellationToken? cancellationToken = null);
}
