using Dapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;

namespace Common.Infrastructure.OutboxScaling;

internal sealed class OutboxProcessorOld(
    NpgsqlDataSource dataSource,
    IPublishEndpoint publishEndpoint,
    ILogger<OutboxProcessorOld> logger)
{
    private const int BatchSize = 1000;

    public async Task<int> Execute(CancellationToken cancellationToken = default)
    {
        Stopwatch totalStopwatch = Stopwatch.StartNew();
        Stopwatch stepStopwatch = new();

        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        stepStopwatch.Restart();
        List<OutboxMessageScale> messages = (await connection.QueryAsync<OutboxMessageScale>(
            """
            SELECT *
            FROM outbox_messages
            WHERE processed_on_utc IS NULL
            ORDER BY occurred_on_utc LIMIT @BatchSize
            """,
            new { BatchSize },
            transaction: transaction)).AsList();
        long queryTime = stepStopwatch.ElapsedMilliseconds;

        ConcurrentQueue<OutboxUpdate> updateQueue = new();

        stepStopwatch.Restart();
        foreach (OutboxMessageScale? message in messages)
        {
            try
            {
                Type? messageType = Application.AssemblyReference.Assembly.GetType(message.Type);
                object? deserializedMessage = JsonSerializer.Deserialize(message.Content, messageType!);

                await publishEndpoint.Publish(deserializedMessage!, messageType!, cancellationToken);

                updateQueue.Enqueue(new OutboxUpdate
                {
                    Id = message.Id,
                    ProcessedOnUtc = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                updateQueue.Enqueue(new OutboxUpdate
                {
                    Id = message.Id,
                    ProcessedOnUtc = DateTime.UtcNow,
                    Error = ex.ToString()
                });
            }
        }
        long publishTime = stepStopwatch.ElapsedMilliseconds;

        stepStopwatch.Restart();
        foreach (OutboxUpdate outboxUpdate in updateQueue)
        {
            await connection.ExecuteAsync(
                """
                UPDATE outbox_messages
                SET processed_on_utc = @ProcessedOnUtc, error = @Error
                WHERE id = @Id
                """,
                outboxUpdate,
                transaction: transaction);
        }
        long updateTime = stepStopwatch.ElapsedMilliseconds;

        await transaction.CommitAsync(cancellationToken);

        totalStopwatch.Stop();
        long totalTime = totalStopwatch.ElapsedMilliseconds;

        OutboxLoggers.LogProcessingPerformance(logger, totalTime, queryTime, publishTime, updateTime, messages.Count);

        return messages.Count;
    }

    private readonly struct OutboxUpdate
    {
        public Guid Id { get; init; }
        public DateTime ProcessedOnUtc { get; init; }
        public string? Error { get; init; }
    }
}
