using FluentValidation;

namespace System.Application.Features.TenantType.CreateTenantType;

internal sealed class CreateTenantTypeValidator : AbstractValidator<CreateTenantTypeCommand>
{
    public CreateTenantTypeValidator()
    {
        RuleFor(p => p.Request.TenantTypeCode)
            .NotEmpty()
            .NotNull();

        RuleFor(p => p.Request.TenantTypeDesc)
            .NotEmpty()
            .NotNull();
    }
}
