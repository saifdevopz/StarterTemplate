using FluentValidation;

namespace System.Application.Features.TenantType.UpdateTenantType;

internal sealed class UpdateTenantTypeValidator : AbstractValidator<UpdateTenantTypeCommand>
{
    public UpdateTenantTypeValidator()
    {
        RuleFor(p => p.Request.TenantTypeCode)
            .NotEmpty()
            .NotNull()
            .MaximumLength(5);

        RuleFor(p => p.Request.TenantTypeDesc)
            .NotEmpty()
            .NotNull();
    }
}
