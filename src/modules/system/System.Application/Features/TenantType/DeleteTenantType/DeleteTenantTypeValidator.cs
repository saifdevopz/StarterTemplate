using FluentValidation;

namespace System.Application.Features.TenantType.DeleteTenantType;

internal sealed class DeleteTenantTypeValidator : AbstractValidator<DeleteTenantTypeCommand>
{
    public DeleteTenantTypeValidator()
    {
        RuleFor(p => p.Request)
            .NotEmpty()
            .NotNull();
    }
}