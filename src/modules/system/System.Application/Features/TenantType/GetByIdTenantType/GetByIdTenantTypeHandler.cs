using Common.Application.Database;
using Common.Domain.TransferObjects.System;
using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantType.GetByIdTenantType;

internal sealed class GetByIdTenantTypeHandler(IRepository<TenantTypeM> genericRepository)
    : IQueryHandler<GetByIdTenantTypeQuery, ReadTenantTypeDto>
{
    public async Task<Result<ReadTenantTypeDto>> Handle(
        GetByIdTenantTypeQuery request,
        CancellationToken cancellationToken)
    {
        TenantTypeM? objs = await genericRepository.GetByIdAsync(request.TenantTypeId);

        ReadTenantTypeDto result = new
        (
            objs.TenantTypeId,
            objs.TenantTypeCode,
            objs.TenantTypeDesc
        );

        return result;
    }
}