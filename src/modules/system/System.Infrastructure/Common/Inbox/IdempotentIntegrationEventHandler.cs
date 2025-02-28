using Common.Application.Database;
using Common.Application.EventBus;
using Common.Infrastructure.Inbox;
using Dapper;
using System.Data.Common;

namespace System.Infrastructure.Common.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
        IIntegrationEventHandler<TIntegrationEvent> decorated,
        IDbConnectionFactory _dbConnectionFactory)
        : IntegrationEventHandler<TIntegrationEvent>
        where TIntegrationEvent : IIntegrationEvent
{
    public override async Task Handle(
            TIntegrationEvent integrationEvent,
            CancellationToken cancellationToken = default)
    {
        using DbConnection connection = await _dbConnectionFactory.OpenSystemConnection();

        InboxMessageConsumer inboxMessageConsumer = new(integrationEvent.Id, decorated.GetType().Name);

        if (await InboxConsumerExistsAsync(connection, inboxMessageConsumer))
        {
            return;
        }

        await decorated.Handle(integrationEvent, cancellationToken);

        await InsertInboxConsumerAsync(connection, inboxMessageConsumer);
    }

    private static async Task<bool> InboxConsumerExistsAsync(
            DbConnection dbConnection,
            InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
                $"""
                SELECT 1
                FROM {SystemConstants.Schema}.inbox_messages_consumers
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
            INSERT INTO {SystemConstants.Schema}.inbox_messages_consumers(InboxMessageId, Name)
            VALUES (@InboxMessageId, @Name)
            """;

        await dbConnection.ExecuteAsync(sql, inboxMessageConsumer);
    }
}
