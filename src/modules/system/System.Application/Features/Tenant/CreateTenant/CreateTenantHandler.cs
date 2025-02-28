using System.Application.Common.Interfaces;
using System.Domain.Features.Tenant;

namespace System.Application.Features.Tenant.CreateTenant;

internal sealed class CreateTenantHandler(ITenantService tenantService)
        : ICommandHandler<CreateTenantCommand, TenantM>
{
    public async Task<Result<TenantM>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        TenantM newTenant = await tenantService.CreateTenant(request.Request, cancellationToken);

        return Result.Success(newTenant);
    }
}
