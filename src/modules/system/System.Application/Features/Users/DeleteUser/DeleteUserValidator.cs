using FluentValidation;

namespace System.Application.Features.Users.DeleteUser;

internal sealed class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(p => p.Request)
            .NotEmpty()
            .NotNull();
    }
}