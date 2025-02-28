namespace Common.Application.OutboxPattern;

public sealed record OrderCreatedIntegrationEventScale(Guid OrderId);
