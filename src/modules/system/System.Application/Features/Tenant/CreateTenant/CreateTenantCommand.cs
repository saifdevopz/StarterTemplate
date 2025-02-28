using Common.Domain.TransferObjects.System;
using System.Domain.Features.Tenant;

namespace System.Application.Features.Tenant.CreateTenant;

public sealed record CreateTenantCommand(CreateTenantDto Request)
    : ICommand<TenantM>;
