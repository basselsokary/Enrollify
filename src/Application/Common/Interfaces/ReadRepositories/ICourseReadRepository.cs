using Application.Common.ReadModels;
using Application.Contracts.Common;
using Application.Features.Courses.Queries;

namespace Application.Common.Interfaces.ReadRepositories;

public interface ICourseReadRepository
{
    Task<GetCourseByIdResponse?> GetCourseByIdAsync(
        Guid courseId,
        CancellationToken cancellationToken);
    Task<PagedResult<GetCoursesResponse>> GetCoursesAsync(
        PagingParameters paging,
        CancellationToken cancellationToken);
    
    Task AddAsync(CourseDocument courseDocument, CancellationToken cancellationToken);
    Task IncrementEnrollmentCountAsync(Guid courseId, CancellationToken cancellationToken);
}
