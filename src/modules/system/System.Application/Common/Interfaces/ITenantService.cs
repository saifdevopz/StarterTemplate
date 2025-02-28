using Common.Domain.TransferObjects.System;
using System.Domain.Features.Tenant;

namespace System.Application.Common.Interfaces;
public interface ITenantService
{
    Task<TenantM> CreateTenant(CreateTenantDto request, CancellationToken cancellationToken = default);
    Task<List<TenantM>> GetParentTenants(CancellationToken cancellationToken = default);
    Task<List<TenantM>> GetStoreTenants(CancellationToken cancellationToken = default);
    Task<TenantM> GetTenant(int tenantId, CancellationToken cancellationToken = default);
    Task<TenantM> UpdateTenant(CancellationToken cancellationToken = default);
    Task<List<TenantM>> DeleteTenant(int tenantId, CancellationToken cancellationToken = default);
}
