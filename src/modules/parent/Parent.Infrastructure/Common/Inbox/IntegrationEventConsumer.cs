using Common.Application.Database;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.Serialization;
using Common.Infrastructure.System;
using Dapper;
using MassTransit;
using Newtonsoft.Json;
using System.Data.Common;

namespace Parent.Infrastructure.Common.Inbox;

internal sealed class IntegrationEventConsumer<TIntegrationEvent>(IDbConnectionFactory _dbConnectionFactory, ISystemService systemService)
        : IConsumer<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        List<TenantDto> tenants = await systemService.GetParentTenants();

        foreach (TenantDto tenant in tenants)
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
                {Constants.Schema}.inbox_messages(id, type, content, occurred_on_utc)
                VALUES (@Id, @Type, @Content, @OccurredOnUtc)
            """;

            using DbConnection connection = await _dbConnectionFactory.OpenPostgreSQLConnection(tenant.ConnectionString);
            await connection.ExecuteAsync(sql, inboxMessage);
        }
    }
}

