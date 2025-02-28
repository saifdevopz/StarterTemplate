using Common.Application.EventBus;

namespace System.IntegrationEvents.Events;

public sealed class UserCreatedIntegrationEvent(
    Guid id,
    DateTime occurredOnUtc,
    Guid userId,
    string email,
    string userName) : IntegrationEvent(id, occurredOnUtc)
{
    public Guid UserId { get; init; } = userId;

    public string Email { get; init; } = email;

    public string Username { get; init; } = userName;

}