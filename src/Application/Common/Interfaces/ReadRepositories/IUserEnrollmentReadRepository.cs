using Application.Common.ReadModels;
using Application.Contracts.Common;
using Application.Features.Enrollments.Queries;

namespace Application.Common.Interfaces.ReadRepositories;

public interface IUserEnrollmentReadRepository
{
    Task<UserEnrollmentDocument?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken);
    Task<UserEnrollmentResponse?> GetEnrollmentByIdAsync(
        Guid userId,
        Guid enrollmentId,
        CancellationToken cancellationToken);
    Task<PagedResult<UserEnrollmentsResponse>> GetEnrollmentsByUserIdAsync(
        Guid userId,
        PagingParameters paging,
        CancellationToken cancellationToken);
    
    Task AddAsync(UserEnrollmentDocument userEnrollmentDocument, CancellationToken cancellationToken);
    Task DeleteAsync(Guid enrollmentId, CancellationToken cancellationToken);
    Task UpdateAsync(UserEnrollmentDocument userEnrollmentDocument, CancellationToken cancellationToken);
}
