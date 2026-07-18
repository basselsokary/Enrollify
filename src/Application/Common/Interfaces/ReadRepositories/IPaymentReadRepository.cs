using Application.Common.ReadModels;
using Application.Contracts.Common;
using Application.Features.Payments.Queries;

namespace Application.Common.Interfaces.ReadRepositories;

public interface IPaymentReadRepository
{
    Task<PaymentDocument?> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken);
    Task<PagedResult<UserPaymentResponse>> GetPaymentsByUserIdAsync(
        Guid userId,
        PagingParameters paging,
        CancellationToken cancellationToken);
    
    Task AddAsync(PaymentDocument paymentDocument, CancellationToken cancellationToken);
    Task UpdateAsync(PaymentDocument updatedPaymentDocument, CancellationToken cancellationToken);
}
