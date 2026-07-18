using System.Text.Json;
using Domain.Common;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Persistence.WriteContext.Interceptors;

internal sealed class DomainEventToOutboxInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var domainEvents = GetDomainEvents(eventData.Context);

        if (domainEvents.Count == 0)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            var eventType = domainEvent.GetType().AssemblyQualifiedName!;
            var eventData = JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), JsonSerializerSettings.Options);

            return OutboxMessage.Create(eventType, eventData);
        }).ToList();

        eventData.Context.Set<OutboxMessage>().AddRange(outboxMessages);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static List<BaseEvent> GetDomainEvents(DbContext context)
    {
        var domainEntities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Count > 0)
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity =>
        {
            entity.ClearDomainEvents();
        });

        return domainEvents;
    }
}