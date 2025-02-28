using Common.Domain.Abstractions;

namespace Store.Domain.Order;

public sealed class OrderCreatedDomainEvent(string orderCode, string orderDesc) : DomainEvent
{
    public string OrderCode { get; init; } = orderCode;
    public string OrderDesc { get; init; } = orderDesc;
}