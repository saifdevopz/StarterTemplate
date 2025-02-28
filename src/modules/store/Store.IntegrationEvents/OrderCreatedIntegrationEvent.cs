using Common.Application.EventBus;

namespace Store.IntegrationEvents;

public sealed class OrderCreatedIntegrationEvent(
    Guid id,
    DateTime occurredOnUtc,
    string orderCode,
    string orderDesc) : IntegrationEvent(id, occurredOnUtc)
{
    public string OrderCode { get; init; } = orderCode;

    public string OrderDesc { get; init; } = orderDesc;
}