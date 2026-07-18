namespace Domain.Common.Interfaces;

public interface IDomainEventHandler<in TEvent>
    where TEvent : BaseEvent
{
    Task HandleAsync(TEvent notification, CancellationToken cancellationToken = default);
}
