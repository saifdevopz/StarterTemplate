using FluentValidation;

namespace Parent.Application.Inventory.CategoryGroup.CreateCategoryGroup;

internal sealed class CreateCategoryGroupValidator : AbstractValidator<CreateCategoryGroupCommand>
{
    public CreateCategoryGroupValidator()
    {
        RuleFor(p => p.Request.CategoryGroupCode)
            .NotEmpty()
            .NotNull();

        RuleFor(p => p.Request.CategoryGroupDesc)
            .NotEmpty()
            .NotNull();
    }
}