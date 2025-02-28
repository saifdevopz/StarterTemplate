using FluentValidation;

namespace System.Application.Features.Tenant.CreateTenant;

internal sealed class CreateTenantValidator : AbstractValidator<CreateTenantCommand>
{
    public CreateTenantValidator()
    {
        RuleFor(p => p.Request.TenantName)
            .NotEmpty()
            .NotNull();

        RuleFor(p => p.Request.DatabaseName)
            .NotEmpty()
            .NotNull();
    }
}
