using Common.Application.Clock;
using Common.Application.Database;
using Common.Application.Messaging;
using Common.Domain.Abstractions;
using Common.Infrastructure.Events;
using Common.Infrastructure.Serialization;
using Common.Infrastructure.System;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using System.Data;
using System.Data.Common;

namespace Store.Infrastructure.Common.Outbox;

[DisallowConcurrentExecution] // To allow only one instance of a background job at a time.
internal sealed class ProcessOutboxJob(
        IDbConnectionFactory _dbConnectionFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeProvider dateTimeProvider,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxJob> logger,
        ISystemService systemService) : IJob
{
    private const string ModuleName = Constants.ModuleName;
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process outbox messages", ModuleName);

        List<TenantDto> tenants = await systemService.GetParentTenants();

        foreach (TenantDto tenant in tenants)
        {
            await using DbConnection connection = await _dbConnectionFactory.OpenSQLServerConnection(tenant.ConnectionString);
            await using DbTransaction transaction = await connection.BeginTransactionAsync();

            // Get unprocessed outbox messages from database.
            IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

            foreach (OutboxMessageResponse outboxMessage in outboxMessages)
            {
                Exception? exception = null;

                try
                {
                    IDomainEvent domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                            outboxMessage.Content,
                            SerializerSettings.Instance)!;

                    using IServiceScope scope = serviceScopeFactory.CreateScope();

                    IEnumerable<IDomainEventHandler> handlers = DomainEventHandlersFactory.GetHandlers(
                            domainEvent.GetType(),
                            scope.ServiceProvider,
                            Application.AssemblyReference.Assembly);

                    foreach (IDomainEventHandler domainEventHandler in handlers)
                    {
                        await domainEventHandler.Handle(domainEvent, context.CancellationToken);
                    }
                }
                catch (Exception caughtException)
                {
                    logger.LogError(
                            caughtException,
                            "{Module} - Exception while processing outbox message {MessageId}",
                            ModuleName,
                            outboxMessage.Id);

                    exception = caughtException;
                }

                await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
            }

            await transaction.CommitAsync();
            logger.LogInformation("{Module} - Successfully processed outbox messages for tenant {TenantId}", ModuleName, tenant.TenantName);
        }

        logger.LogInformation("{Module} - Completed processing outbox messages", ModuleName);
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
            IDbConnection connection,
            IDbTransaction transaction)
    {

        string sql =
            $"""
            SELECT TOP {outboxOptions.Value.BatchSize}
                id AS {nameof(OutboxMessageResponse.Id)},
                content AS {nameof(OutboxMessageResponse.Content)}
            FROM "{Constants.Schema}".outbox_messages
            WHERE ProcessedOnUtc IS NULL
            ORDER BY OccurredOnUtc
            """;

        IEnumerable<OutboxMessageResponse> outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(
                sql,
                transaction: transaction);

        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            OutboxMessageResponse outboxMessage,
            Exception? exception)
    {

        const string sql =
        $"""
            UPDATE {Constants.Schema}.outbox_messages
            SET ProcessedOnUtc = @ProcessedOnUtc,
                Error = @Error
            WHERE Id = @Id
         """;

        await connection.ExecuteAsync(
                sql,
                new
                {
                    outboxMessage.Id,
                    ProcessedOnUtc = dateTimeProvider.UtcNow,
                    Error = exception?.ToString()
                },
                transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
