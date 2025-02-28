using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.Infrastructure.OutboxScaling;

internal sealed class OutboxBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OutboxBackgroundService> logger) : BackgroundService
{
    private const int OutboxProcessorFrequency = 5;
    private readonly int _maxParallelism = 5;
    private int _totalIterations;
    private int _totalProcessedMessage;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        OutboxLoggers.LogStarting(logger);

        using CancellationTokenSource cts = new(TimeSpan.FromMinutes(10));
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, stoppingToken);

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = _maxParallelism,
            CancellationToken = linkedCts.Token
        };

        try
        {
            await Parallel.ForEachAsync(
                Enumerable.Range(0, _maxParallelism),
                parallelOptions,
                async (_, token) =>
                {
                    await ProcessOutboxMessages(token);
                });
        }
        catch (OperationCanceledException)
        {
            OutboxLoggers.LogOperationCancelled(logger);
        }
        catch (Exception ex)
        {
            OutboxLoggers.LogError(logger, ex);
        }
        finally
        {
            OutboxLoggers.LogFinished(logger, _totalIterations, _totalProcessedMessage);
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        OutboxProcessor outboxProcessor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();

        while (!cancellationToken.IsCancellationRequested)
        {
            int iterationCount = Interlocked.Increment(ref _totalIterations);
            OutboxLoggers.LogStartingIteration(logger, iterationCount);

            int processedMessages = await outboxProcessor.Execute(cancellationToken);
            int totalProcessedMessages = Interlocked.Add(ref _totalProcessedMessage, processedMessages);

            OutboxLoggers.LogIterationCompleted(logger, iterationCount, processedMessages, totalProcessedMessages);

            // Simulate running Outbox processing every N seconds
            await Task.Delay(TimeSpan.FromSeconds(OutboxProcessorFrequency), cancellationToken);
        }
    }
}