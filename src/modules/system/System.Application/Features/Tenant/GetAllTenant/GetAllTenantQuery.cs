using Common.Domain.TransferObjects.System;

namespace System.Application.Features.Tenant.GetAllTenant;

public sealed record GetAllTenantQuery : IQuery<List<GetAllTenants>>;