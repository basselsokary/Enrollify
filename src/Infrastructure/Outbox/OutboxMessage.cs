namespace Infrastructure.Outbox;

internal sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; } = null!;
    public string Data { get; private set; } = null!;
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; private set; }
    public string? Error { get; private set; }
    
    private OutboxMessage() { }

    internal static OutboxMessage Create(string type, string data)
    {
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = type,
            Data = data,
            OccurredOn = DateTime.UtcNow,
            ProcessedOn = null,
            Error = null
        };
    }

    internal void MarkAsProcessed(DateTime dateTime)
    {
        ProcessedOn = dateTime;
    }

    internal void MarkAsFailed(DateTime dateTime, string error)
    {
        ProcessedOn = dateTime;
        Error = error;
    }
}
