using Common.Application.EventBus;

namespace Parent.IntegrationEvents;

public sealed class CreateCategoryGroupIntegrationEvent(
    Guid id,
    DateTime occurredOnUtc,
    string categoryGroupCode,
    string categoryGroupDesc) : IntegrationEvent(id, occurredOnUtc)
{
    public string CategoryGroupCode { get; init; } = categoryGroupCode;

    public string CategoryGroupDesc { get; init; } = categoryGroupDesc;
}