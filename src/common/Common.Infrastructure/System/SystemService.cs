using Dapper;
using Microsoft.Data.SqlClient;

namespace Common.Infrastructure.System;
internal sealed class SystemService(CurrentTenant currentTenant) : ISystemService
{
    public async Task<List<T>> GetAllEntities<T>(string sqlQuery, CancellationToken? cancellationToken = null)
    {
        if (string.IsNullOrEmpty(CommonConstants.SystemConnectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        using SqlConnection connection = new(CommonConstants.SystemConnectionString);
        await connection.OpenAsync(cancellationToken ?? CancellationToken.None);

        IEnumerable<T> entities = await connection.QueryAsync<T>(sqlQuery);

        return entities.ToList();
    }

    public async Task<List<TenantDto>> GetParentTenants(CancellationToken? cancellationToken = null)
    {
        string sqlQuery = "SELECT * FROM Main.Tenants WHERE TenantTypeId = 2";

        using SqlConnection connection = new(currentTenant.GetSystemConnectionString);
        await connection.OpenAsync(cancellationToken ?? CancellationToken.None);
        IEnumerable<TenantDto> entities = await connection.QueryAsync<TenantDto>(sqlQuery);

        return entities.ToList();
    }
}
