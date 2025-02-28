using Common.Application.Database;
using Common.Application.Messaging;
using Common.Domain.Abstractions;
using Common.Infrastructure.Outbox;
using Common.Infrastructure.System;
using Dapper;
using System.Data.Common;

namespace Store.Infrastructure.Common.Outbox;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
        IDomainEventHandler<TDomainEvent> decorated,
        IDbConnectionFactory _dbConnectionFactory,
        ISystemService systemService)
        : DomainEventHandler<TDomainEvent>
        where TDomainEvent : IDomainEvent
{
    public override async Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        List<TenantDto> tenants = await systemService.GetParentTenants(cancellationToken);

        foreach (TenantDto tenant in tenants)
        {
            await using DbConnection connection = await _dbConnectionFactory.OpenSQLServerConnection(tenant.ConnectionString);

            OutboxMessageConsumer outboxMessageConsumer = new(domainEvent.Id, decorated.GetType().Name);

            if (await OutboxConsumerExistsAsync(connection, outboxMessageConsumer))
            {
                return;
            }

            await decorated.Handle(domainEvent, cancellationToken);

            await InsertOutboxConsumerAsync(connection, outboxMessageConsumer);
        }
    }

    private static async Task<bool> OutboxConsumerExistsAsync(
            DbConnection dbConnection,
            OutboxMessageConsumer outboxMessageConsumer)
    {
        const string sql =
            $"""
                SELECT 1
                FROM {Constants.Schema}.outbox_message_consumers
                WHERE OutboxMessageId = @OutboxMessageId AND
                        Name = @Name
            """;

        return await dbConnection.ExecuteScalarAsync<bool>(sql, outboxMessageConsumer);
    }

    private static async Task InsertOutboxConsumerAsync(
            DbConnection dbConnection,
            OutboxMessageConsumer outboxMessageConsumer)
    {
        const string sql =
            $"""
                INSERT INTO {Constants.Schema}.outbox_message_consumers(OutboxMessageId, Name)
                VALUES (@OutboxMessageId, @Name)
            """;

        await dbConnection.ExecuteAsync(sql, outboxMessageConsumer);
    }
}
