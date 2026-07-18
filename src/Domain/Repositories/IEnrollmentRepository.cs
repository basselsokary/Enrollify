using Domain.Entities.EnrollmentAggregate;

namespace Domain.Repositories;

public interface IEnrollmentRepository : IBaseRepository<Enrollment>
{
    Task<Enrollment?> GetByEnrollmentIdAndUserIdAsync(Guid enrollmentId, Guid userId, CancellationToken cancellationToken);
    Task<bool> IsUserEnrolledAsync(Guid courseId, Guid userId, CancellationToken cancellationToken);
}
