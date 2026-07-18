using System.Text.Json;
using Domain.Common;
using Infrastructure.Events;
using Infrastructure.Outbox;
using Infrastructure.Persistence.WriteContext.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundJobs;

internal sealed class OutboxBackgroundService(
    IServiceScopeFactory scopeFactory,
    OutboxChannel outboxChannel,
    ILogger<OutboxBackgroundService> logger,
    IOptions<OutboxOptions> outboxOptions) : BackgroundService
{
    private readonly OutboxOptions _options = outboxOptions.Value;
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        logger.LogInformation("OutboxBackgroundService is starting...");

        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    timeoutCts.CancelAfter(TimeSpan.FromMinutes(_options.FrequencyInMinutes));
                    // timeoutCts.CancelAfter(TimeSpan.FromSeconds(10)); // for testing purposes, set to 10 seconds
                    await outboxChannel.WaitAsync(timeoutCts.Token);
                }
                catch (OperationCanceledException) when (!ct.IsCancellationRequested)
                {
                    // This means the wait was canceled due to timeout, not because the service is stopping.
                }
            
                await ProcessOutboxMessages(ct);
            }
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
            logger.LogInformation("OutboxBackgroundService is stopping due to cancellation.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred in OutboxBackgroundService.");
        }
    }

    private async Task ProcessOutboxMessages(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<DomainEventDispatcher>();
        
        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        foreach (var msg in messages)
        {
            var eventType = Type.GetType(msg.Type)!;
            var deserializedEvent = (BaseEvent) JsonSerializer.Deserialize(
                msg.Data,
                eventType,
                JsonSerializerSettings.Options)!;

            await dispatcher.DispatchAsync([deserializedEvent], ct); // Dispatch the event to its handlers

            msg.MarkAsProcessed(DateTime.UtcNow);
        }

        await dbContext.SaveChangesAsync(ct);
    }
}

public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    public int FrequencyInMinutes { get; init; } = 2;
    public int BatchSize { get; init; } = 30;
}
