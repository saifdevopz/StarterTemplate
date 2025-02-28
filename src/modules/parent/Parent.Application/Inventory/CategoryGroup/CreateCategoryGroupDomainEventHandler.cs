using Common.Application.EventBus;
using Common.Application.Messaging;
using Parent.Domain.Inventory.CategoryGroup;
using Parent.IntegrationEvents;

namespace Parent.Application.Inventory.CategoryGroup;

internal sealed class CreateCategoryGroupDomainEventHandler(IEventBus bus)
    : DomainEventHandler<CategoryGroupCreatedDomainEvent>
{
    public override async Task Handle(
        CategoryGroupCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        await bus.PublishAsync(new CreateCategoryGroupIntegrationEvent
        (
            domainEvent.Id,
            domainEvent.OccurredOnUtc,
            domainEvent.CategoryGroupCode,
            domainEvent.CategoryGroupDesc
        ),
            cancellationToken);
    }
}