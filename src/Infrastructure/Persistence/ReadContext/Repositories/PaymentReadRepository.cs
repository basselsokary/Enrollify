using Application.Common.Interfaces.ReadRepositories;
using Application.Common.ReadModels;
using Application.Contracts.Common;
using Application.Features.Payments.Queries;
using MongoDB.Driver;

namespace Infrastructure.Persistence.ReadContext.Repositories;

internal sealed class PaymentReadRepository(MongoDbContext context) : IPaymentReadRepository
{
    private readonly IMongoCollection<PaymentDocument> _collection
        = context.GetCollection<PaymentDocument>("payments");

    public async Task AddAsync(PaymentDocument paymentDocument, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(paymentDocument, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(PaymentDocument updatedPaymentDocument, CancellationToken cancellationToken)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == updatedPaymentDocument.Id,
            updatedPaymentDocument,
            cancellationToken: cancellationToken);
    }

    public async Task<PaymentDocument?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken)
    {
        return await _collection.Find(x => x.Id == paymentId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<UserPaymentResponse>> GetPaymentsByUserIdAsync(
        Guid userId,
        PagingParameters paging,
        CancellationToken cancellationToken)
    {
        var totalCount = await _collection.CountDocumentsAsync(x => x.UserId == userId, cancellationToken: cancellationToken);
        var payments = await _collection.Find(x => x.UserId == userId)
            .SortByDescending(x => x.CreatedAt)
            .Skip(paging.Skip)
            .Limit(paging.Take)
            .Project(x => new UserPaymentResponse(
                x.Id,
                x.UserId,
                x.CourseTitle,
                x.PaymentMethod,
                x.Status,
                x.PaidAt,
                x.PaidAmount,
                x.AmountRefunded))
            .ToListAsync(cancellationToken);
        
        return new PagedResult<UserPaymentResponse>(
            payments,
            (int) totalCount,
            paging.Page,
            paging.PageSize);
    }
}
