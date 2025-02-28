using Common.Domain.Features.Order;

namespace Store.Domain.Order;
public sealed class OrderM : ProductBase
{
    public int OrderId { get; }
    public string OrderCode { get; private set; } = string.Empty;
    public string OrderDesc { get; private set; } = string.Empty;

    public static OrderM Create
    (
        string orderCode,
        string orderDesc
    )
    {
        OrderM order = new()
        {
            OrderCode = orderCode,
            OrderDesc = orderDesc,
        };

        order.AddDomainEvent(new OrderCreatedDomainEvent(order.OrderCode, order.OrderDesc));

        return order;
    }

}