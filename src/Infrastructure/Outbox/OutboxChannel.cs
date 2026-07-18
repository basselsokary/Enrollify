namespace Infrastructure.Outbox;

// A lightweight signal — no DB, no queue, just a semaphore
public sealed class OutboxChannel
{
    private readonly SemaphoreSlim _signal = new(0);

    public void Notify() => _signal.Release();  // called after SaveChanges
    public Task WaitAsync(CancellationToken ct) => _signal.WaitAsync(ct); // blocks until signaled
}
