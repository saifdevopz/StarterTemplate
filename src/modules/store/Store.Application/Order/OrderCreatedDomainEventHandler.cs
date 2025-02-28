using Common.Application.EventBus;
using Common.Application.Messaging;
using Store.Domain.Order;
using Store.IntegrationEvents;

namespace Store.Application.Order;

internal sealed class OrderCreatedDomainEventHandler(IEventBus bus)
    : DomainEventHandler<OrderCreatedDomainEvent>
{
    public override async Task Handle(
        OrderCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await bus.PublishAsync(new OrderCreatedIntegrationEvent
        (
            domainEvent.Id,
            domainEvent.OccurredOnUtc,
            domainEvent.OrderCode,
            domainEvent.OrderDesc
        ),
            cancellationToken);
    }
}