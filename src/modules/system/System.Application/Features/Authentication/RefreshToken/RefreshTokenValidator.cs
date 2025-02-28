using FluentValidation;

namespace System.Application.Features.Authentication.RefreshToken;

internal sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(p => p.Request.Token)
            .NotEmpty()
            .NotNull();

        RuleFor(p => p.Request.RefreshToken)
            .NotEmpty()
            .NotNull();
    }
}