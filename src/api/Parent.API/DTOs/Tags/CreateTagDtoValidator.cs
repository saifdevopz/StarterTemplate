using FluentValidation;

namespace Parent.API.DTOs.Tags;

internal sealed class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
{
    public CreateTagDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MinimumLength(3);

        RuleFor(x => x.Description).MaximumLength(50);
    }
}
