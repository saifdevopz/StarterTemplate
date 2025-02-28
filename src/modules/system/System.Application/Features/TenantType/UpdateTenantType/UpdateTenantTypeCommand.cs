using Common.Domain.TransferObjects.System;

namespace System.Application.Features.TenantType.UpdateTenantType;

public sealed record UpdateTenantTypeCommand(WriteTenantTypeDto Request)
    : ICommand<bool>;
