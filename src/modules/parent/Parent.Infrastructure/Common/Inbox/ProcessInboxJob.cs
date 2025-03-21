﻿using Common.Application.Clock;
using Common.Application.Database;
using Common.Application.EventBus;
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

namespace Parent.Infrastructure.Common.Inbox;

[DisallowConcurrentExecution]
internal sealed class ProcessInboxJob(
        IDbConnectionFactory _dbConnectionFactory,
        IServiceScopeFactory serviceScopeFactory,
        IDateTimeProvider dateTimeProvider,
        IOptions<InboxOptions> inboxOptions,
        ILogger<ProcessInboxJob> logger,
        ISystemService systemService) : IJob
{
    private const string ModuleName = Constants.ModuleName;

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process inbox messages", ModuleName);

        List<TenantDto> tenants = await systemService.GetParentTenants(null);

        foreach (TenantDto tenant in tenants)
        {
            await using DbConnection connection = await _dbConnectionFactory.OpenPostgreSQLConnection(tenant.ConnectionString);

            await using DbTransaction transaction = await connection.BeginTransactionAsync();

            IReadOnlyList<InboxMessageResponse> inboxMessages = await GetInboxMessagesAsync(connection, transaction);

            foreach (InboxMessageResponse inboxMessage in inboxMessages)
            {
                Exception? exception = null;

                try
                {
                    IIntegrationEvent integrationEvent = JsonConvert.DeserializeObject<IIntegrationEvent>(
                            inboxMessage.Content,
                            SerializerSettings.Instance)!;

                    using IServiceScope scope = serviceScopeFactory.CreateScope();

                    IEnumerable<IIntegrationEventHandler> handlers = IntegrationEventHandlersFactory.GetHandlers(
                            integrationEvent.GetType(),
                            scope.ServiceProvider,
                            Presentation.AssemblyReference.Assembly);

                    foreach (IIntegrationEventHandler integrationEventHandler in handlers)
                    {
                        await integrationEventHandler.Handle(integrationEvent, context.CancellationToken);
                    }
                }
                catch (Exception caughtException)
                {
                    logger.LogError(
                            caughtException,
                            "{Module} - Exception while processing inbox message {MessageId}",
                            ModuleName,
                            inboxMessage.Id);

                    exception = caughtException;
                }

                await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
            }

            await transaction.CommitAsync();
        }
        logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
    }

    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(
            IDbConnection connection,
            IDbTransaction transaction)
    {

        string sql =
            $"""
                 SELECT
                    id AS {nameof(InboxMessageResponse.Id)},
                    content AS {nameof(InboxMessageResponse.Content)}
                 FROM {Constants.Schema}.inbox_messages
                 WHERE processed_on_utc IS NULL
                 ORDER BY occurred_on_utc
                 LIMIT {inboxOptions.Value.BatchSize}
                 FOR UPDATE
            """;

        IEnumerable<InboxMessageResponse> inboxMessages = await connection.QueryAsync<InboxMessageResponse>(
                sql,
                transaction: transaction);

        return inboxMessages.AsList();
    }

    private async Task UpdateInboxMessageAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            InboxMessageResponse inboxMessage,
            Exception? exception)
    {
        const string sql =
            $"""
                UPDATE {Constants.Schema}.inbox_messages
                SET processed_on_utc = @ProcessedOnUtc,
                    error = @Error
                WHERE id = @Id
            """;

        await connection.ExecuteAsync(
                sql,
                new
                {
                    inboxMessage.Id,
                    ProcessedOnUtc = dateTimeProvider.UtcNow,
                    Error = exception?.ToString()
                },
                transaction: transaction);
    }

    internal sealed record InboxMessageResponse(Guid Id, string Content);
}
