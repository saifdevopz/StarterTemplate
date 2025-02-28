using Common.Domain.TransferObjects.System;

namespace System.Application.Features.TenantType.GetByIdTenantType;

public sealed record GetByIdTenantTypeQuery(int TenantTypeId) : IQuery<ReadTenantTypeDto>;