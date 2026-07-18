using Application.Common.Interfaces.ReadRepositories;
using Application.Common.ReadModels;
using Application.Contracts.Common;
using Application.Features.Enrollments.Queries;
using Domain.Entities.EnrollmentAggregate;
using MongoDB.Driver;

namespace Infrastructure.Persistence.ReadContext.Repositories;

internal sealed class UserEnrollmentReadRepository(MongoDbContext context) : IUserEnrollmentReadRepository
{
    private readonly IMongoCollection<UserEnrollmentDocument> _collection
        = context.GetCollection<UserEnrollmentDocument>("user_enrollments");

    public async Task AddAsync(UserEnrollmentDocument userEnrollmentDocument, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(userEnrollmentDocument, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        var filter = Builders<UserEnrollmentDocument>.Filter.Eq(x => x.Id, enrollmentId);
        await _collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task UpdateAsync(UserEnrollmentDocument userEnrollmentDocument, CancellationToken cancellationToken)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == userEnrollmentDocument.Id,
            userEnrollmentDocument,
            cancellationToken: cancellationToken);
    }

    public Task UpdateStatusAsync(Guid enrollmentId, EnrollmentStatus status, CancellationToken cancellationToken)
    {
        var update = Builders<UserEnrollmentDocument>.Update.Set(x => x.Status, status);
        return _collection.UpdateOneAsync(x => x.Id == enrollmentId, update, cancellationToken: cancellationToken);
    }

    public async Task<UserEnrollmentDocument?> GetByIdAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        return await _collection.Find(x => x.Id == enrollmentId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserEnrollmentResponse?> GetEnrollmentByIdAsync(Guid userId, Guid enrollmentId, CancellationToken cancellationToken)
    {
        return await _collection.Find(x => x.UserId == userId && x.Id == enrollmentId)
            .Project(x => new UserEnrollmentResponse(
                x.Id,
                x.UserId,
                x.CourseId,
                x.CourseTitle,
                x.Status,
                x.EnrollmentAt,
                x.PaidAmount,
                x.ExpiresAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<UserEnrollmentsResponse>> GetEnrollmentsByUserIdAsync(
        Guid userId,
        PagingParameters paging,
        CancellationToken cancellationToken)
    {
        var totalCount = await _collection.CountDocumentsAsync(x => x.UserId == userId, cancellationToken: cancellationToken);
        var enrollments = await _collection.Find(x => x.UserId == userId)
            .SortByDescending(x => x.EnrollmentAt)
            .Skip(paging.Skip)
            .Limit(paging.Take)
            .Project(x => new UserEnrollmentsResponse(
                x.Id,
                x.UserId,
                x.CourseId,
                x.CourseTitle,
                x.Status,
                x.EnrollmentAt,
                x.PaidAmount,
                x.ExpiresAt))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserEnrollmentsResponse>(
            enrollments,
            (int) totalCount,
            paging.Page,
            paging.PageSize);
    }
}
