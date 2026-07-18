using Domain.Common;
using Domain.Common.Shared;
using Domain.Repositories;
using Infrastructure.Outbox;
using Infrastructure.Persistence.WriteContext.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.WriteContext.Repositories;

internal sealed class UnitOfWork(
    WriteDbContext context,
    OutboxChannel outboxChannel) : IUnitOfWork
{
    private bool _disposed = false;

    private ICourseRepository? _courseRepository;
    private IEnrollmentRepository? _enrollmentRepository;
    private IPaymentRepository? _paymentRepository;

    public ICourseRepository Courses
        => _courseRepository ??= new CourseRepository(context);
    public IEnrollmentRepository Enrollments
        => _enrollmentRepository ??= new EnrollmentRepository(context);
    public IPaymentRepository Payments
        => _paymentRepository ??= new PaymentRepository(context);

    public Task UpdateAsync<T>(T entity) where T : BaseEntity
    {
        context.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync<T>(T entity) where T : BaseEntity
    {
        context.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        await context.AddAsync(entity, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await context.SaveChangesAsync(cancellationToken);
        outboxChannel.Notify(); // Notify the OutboxBackgroundService that there are new messages to process
        return result;
    }

    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            await operation();
            return true; // dummy result
        }, cancellationToken);
    }
    
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await operation();
                
                if (result is Result { Failed: true })
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return result;
                }

                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            context.Dispose();
            _disposed = true;
        }
    }
}
