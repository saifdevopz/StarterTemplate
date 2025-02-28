using Common.Application.Database;
using Common.Domain.TransferObjects.System;
using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantType.GetAllTenantType;

internal sealed class GetAllTenantTypeHandler(IRepository<TenantTypeM> genericRepository)
    : IQueryHandler<GetAllTenantTypeQuery, List<ReadTenantTypeDto>>
{
    public async Task<Result<List<ReadTenantTypeDto>>> Handle(
        GetAllTenantTypeQuery request,
        CancellationToken cancellationToken)
    {
        List<TenantTypeM> objects = await genericRepository.GetAllAsync();

        List<ReadTenantTypeDto> result = objects.Select(obj => new ReadTenantTypeDto
        (
            obj.TenantTypeId,
            obj.TenantTypeCode,
            obj.TenantTypeDesc
        )).ToList();

        return result;
    }
}