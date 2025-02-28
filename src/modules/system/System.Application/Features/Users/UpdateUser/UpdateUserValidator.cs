using FluentValidation;

namespace System.Application.Features.Users.UpdateUser;

internal sealed class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(p => p.Request.Email)
            .NotEmpty()
            .NotNull()
            .MaximumLength(5);

        RuleFor(p => p.Request.Password)
            .NotEmpty()
            .NotNull();
    }
}