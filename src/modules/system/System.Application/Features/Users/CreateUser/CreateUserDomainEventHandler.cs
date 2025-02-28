using Common.Application.EventBus;
using Serilog;
using System.Domain.Features.Identity.Events;
using System.IntegrationEvents.Events;

namespace System.Application.Features.Users.CreateUser;

internal sealed class CreateUserDomainEventHandler(/*ISender sender*/ IEventBus bus)
    : DomainEventHandler<UserCreatedDomainEvent>
{
    public override async Task Handle(
        UserCreatedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        Log.Information("sbfgbvfffffffffffffffff");

        await bus.PublishAsync(
            new UserCreatedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.UserId,
                "saif@gmail.com",
                "Saif"),
            cancellationToken);
    }
}