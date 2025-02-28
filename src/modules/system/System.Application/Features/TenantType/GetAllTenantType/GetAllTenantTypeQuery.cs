using Common.Domain.TransferObjects.System;

namespace System.Application.Features.TenantType.GetAllTenantType;

public sealed record GetAllTenantTypeQuery : IQuery<List<ReadTenantTypeDto>>;