using Domain.Common;

namespace Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    /// Repository access
    ICourseRepository Courses { get; }
    IEnrollmentRepository Enrollments { get; }
    IPaymentRepository Payments { get; }

    /// Generic operations; just changing the entity state in the context, not saving changes to the database
    Task UpdateAsync<T>(T entity) where T : BaseEntity;
    Task DeleteAsync<T>(T entity) where T : BaseEntity;
    Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// Transaction management
    Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default);
    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}
