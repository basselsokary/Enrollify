using Application.Common.Interfaces.ReadRepositories;
using Application.Common.ReadModels;
using Application.Contracts.Common;
using Application.Features.Courses.Queries;
using MongoDB.Driver;

namespace Infrastructure.Persistence.ReadContext.Repositories;

internal sealed class CourseReadRepository(MongoDbContext context) : ICourseReadRepository
{
    private readonly IMongoCollection<CourseDocument> _collection
        = context.GetCollection<CourseDocument>("courses");

    public async Task AddAsync(CourseDocument courseDocument, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(courseDocument, cancellationToken: cancellationToken);
    }

    public async Task IncrementEnrollmentCountAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var update = Builders<CourseDocument>.Update.Inc(x => x.EnrolledCount, 1);

        await _collection.UpdateOneAsync(x => x.Id == courseId, update, cancellationToken: cancellationToken);
    }

    public async Task<GetCourseByIdResponse?> GetCourseByIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var course = await _collection.Find(x => x.Id == courseId)
            .Project(x => new GetCourseByIdResponse(
                x.Id,
                x.Title,
                x.Description,
                x.DurationInMinutes,
                x.Price,
                x.Currency,
                x.IsFree,
                x.CreatedAt,
                x.LastModifiedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return course;
    }

    public async Task<PagedResult<GetCoursesResponse>> GetCoursesAsync(PagingParameters paging, CancellationToken cancellationToken)
    {
        var totalCount = await _collection.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
        var courses = await _collection.Find(_ => true)
            .SortByDescending(x => x.CreatedAt)
            .Skip(paging.Skip)
            .Limit(paging.Take)
            .Project(x => new GetCoursesResponse(
                x.Id,
                x.Title,
                x.Description,
                x.Price,
                x.Currency,
                x.IsFree,
                x.DurationInMinutes,
                x.EnrolledCount,
                x.CreatedAt,
                x.LastModifiedAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<GetCoursesResponse>(
            courses,
            (int) totalCount,
            paging.Page,
            paging.PageSize);
    }
}
