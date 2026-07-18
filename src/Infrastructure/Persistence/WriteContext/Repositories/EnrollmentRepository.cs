using Domain.Entities.EnrollmentAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.WriteContext.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.WriteContext.Repositories;

internal sealed class EnrollmentRepository(WriteDbContext context)
    : BaseRepository<Enrollment>(context), IEnrollmentRepository
{
    public Task<Enrollment?> GetByEnrollmentIdAndUserIdAsync(Guid enrollmentId, Guid userId, CancellationToken cancellationToken)
    {
        return DbContext.Enrollments
            .FirstOrDefaultAsync(e => e.Id == enrollmentId && e.UserId == userId, cancellationToken);
    }

    public async Task<bool> IsUserEnrolledAsync(Guid courseId, Guid userId, CancellationToken cancellationToken)
    {
        return await DbContext.Enrollments
            .AnyAsync(
                e => e.CourseId == courseId
                    && e.UserId == userId,
                cancellationToken);
    }
}
