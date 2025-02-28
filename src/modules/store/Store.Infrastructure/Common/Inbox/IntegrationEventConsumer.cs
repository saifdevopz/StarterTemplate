using Common.Application.Database;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Serialization;
using MassTransit;
using Newtonsoft.Json;

namespace Store.Infrastructure.Common.Inbox;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(IDbConnectionFactory _dbConnectionFactory)
        : IConsumer<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        TIntegrationEvent integrationEvent = context.Message;

        InboxMessage inboxMessage = new()
        {
            Id = integrationEvent.Id,
            Type = integrationEvent.GetType().Name,
            Content = JsonConvert.SerializeObject(integrationEvent, SerializerSettings.Instance),
            OccurredOnUtc = integrationEvent.OccurredOnUtc
        };

        const string sql =
            $"""
                INSERT INTO 
                {Constants.Schema}.InboxMessages(id, type, content, occurred_on_utc)
                VALUES (@Id, @Type, @Content, @OccurredOnUtc)
            """;

        await _dbConnectionFactory.ExecuteAsync(sql, inboxMessage);
    }
}
