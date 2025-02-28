namespace System.Application.Features.TenantType.DeleteTenantType;

public sealed record DeleteTenantTypeCommand(int Request)
    : ICommand<bool>;