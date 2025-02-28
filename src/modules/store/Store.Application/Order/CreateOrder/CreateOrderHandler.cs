using Common.Application.Database;
using Common.Application.Messaging;
using Common.Domain.Results;
using Store.Domain.Order;

namespace Store.Application.Order.CreateOrder;

internal sealed class CreateOrderHandler(IRepository<OrderM> _repository)
        : ICommandHandler<CreateOrderCommand, OrderM>
{
    public async Task<Result<OrderM>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        OrderM obj = OrderM.Create
        (
            request.Request.OrderCode,
            request.Request.OrderDesc
        );

        await _repository.AddAsync(obj);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(obj);
    }
}