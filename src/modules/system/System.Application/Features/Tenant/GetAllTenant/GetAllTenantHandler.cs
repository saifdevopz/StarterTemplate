using Common.Application.Database;
using Common.Domain.TransferObjects.System;
using System.Domain.Features.Tenant;

namespace System.Application.Features.Tenant.GetAllTenant;

internal sealed class GetAllTenantHandler(IRepository<TenantM> genericRepository)
    : IQueryHandler<GetAllTenantQuery, List<GetAllTenants>>
{
    public async Task<Result<List<GetAllTenants>>> Handle(
        GetAllTenantQuery request,
        CancellationToken cancellationToken)
    {
        List<TenantM> tenants = await genericRepository.GetAllAsync();

        List<GetAllTenants> tenantDtos = tenants.Select(tenant => new GetAllTenants(
            tenant.DatabaseName,
            tenant.TenantName
            )).ToList();

        return tenantDtos;
    }
}