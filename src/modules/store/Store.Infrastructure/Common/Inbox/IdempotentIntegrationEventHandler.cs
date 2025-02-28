using Common.Application.Database;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Common.Infrastructure.System;
using Dapper;
using System.Data.Common;

namespace Store.Infrastructure.Common.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
        IIntegrationEventHandler<TIntegrationEvent> decorated,
        IDbConnectionFactory _dbConnectionFactory,
        ISystemService systemService)
        : IntegrationEventHandler<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
{
    public override async Task Handle(
            TIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default)
    {
        List<TenantDto> tenants = await systemService.GetParentTenants(cancellationToken);

        foreach (TenantDto tenant in tenants)
        {
            await using DbConnection connection = await _dbConnectionFactory.OpenSQLServerConnection(tenant.ConnectionString);

            InboxMessageConsumer inboxMessageConsumer = new(integrationEvent.Id, decorated.GetType().Name);

            if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer))
            {
                return;
            }

            await decorated.Handle(integrationEvent, cancellationToken);

            await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
        }
    }

    private static async Task<bool> InboxConsumerExistsAsync(
            DbConnection dbConnection,
            InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
           $"""
                SELECT 1
                FROM {Constants.Schema}.InboxMessageConsumers
                WHERE InboxMessageId = @InboxMessageId AND
                      Name = @Name
            """;

        return await dbConnection.ExecuteScalarAsync<bool>(sql, inboxMessageConsumer);
    }

    private static async Task InsertInboxConsumerAsync(
            DbConnection dbConnection,
            InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
            $"""
                INSERT INTO 
                {Constants.Schema}.InboxMessageConsumers(InboxMessageId, Name)
                VALUES (@InboxMessageId, @Name)
            """;

        await dbConnection.ExecuteAsync(sql, inboxMessageConsumer);
    }
}
