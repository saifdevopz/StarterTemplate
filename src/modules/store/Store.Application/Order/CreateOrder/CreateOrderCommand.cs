using Common.Application.Messaging;
using Common.Domain.TransferObjects.Order;
using Store.Domain.Order;

namespace Store.Application.Order.CreateOrder;

public sealed record CreateOrderCommand(WriteOrder Request)
    : ICommand<OrderM>;