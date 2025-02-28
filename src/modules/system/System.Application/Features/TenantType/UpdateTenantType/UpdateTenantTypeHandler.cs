using Common.Application.Database;
using System.Domain.Features.Tenant;

namespace System.Application.Features.TenantType.UpdateTenantType;

internal sealed class UpdateTenantTypeHandler(IRepository<TenantTypeM> _repository)
        : ICommandHandler<UpdateTenantTypeCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateTenantTypeCommand request, CancellationToken cancellationToken)
    {
        TenantTypeM? tenantTypeM = await _repository.GetByIdAsync(request.Request.TenantTypeId);

        tenantTypeM.TenantTypeCode = request.Request.TenantTypeCode;
        tenantTypeM.TenantTypeDesc = request.Request.TenantTypeDesc;

        _repository.Update(tenantTypeM);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}
