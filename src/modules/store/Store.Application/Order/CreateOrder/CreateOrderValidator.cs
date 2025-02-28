using FluentValidation;

namespace Store.Application.Order.CreateOrder;

internal sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator()
    {
        RuleFor(p => p.Request.OrderCode)
            .NotEmpty()
            .NotNull();

        RuleFor(p => p.Request.OrderDesc)
            .NotEmpty()
            .NotNull();
    }
}