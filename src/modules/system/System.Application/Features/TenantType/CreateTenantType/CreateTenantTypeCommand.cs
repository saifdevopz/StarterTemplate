using Common.Domain.TransferObjects.System;

namespace System.Application.Features.TenantType.CreateTenantType;

public sealed record CreateTenantTypeCommand(WriteTenantTypeDto Request)
    : ICommand<bool>;
