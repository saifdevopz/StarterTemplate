using FluentValidation;

namespace System.Application.Features.Users.CreateUser;

internal sealed class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(c => c.Request.TenantId)
            .NotEmpty();

        RuleFor(c => c.Request.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(c => c.Request.Password)
            .MinimumLength(6)
            .NotEmpty();
    }
}
