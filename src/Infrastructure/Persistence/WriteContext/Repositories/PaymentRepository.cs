using Domain.Entities.PaymentAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.WriteContext.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.WriteContext.Repositories;

internal sealed class PaymentRepository(WriteDbContext context)
    : BaseRepository<Payment>(context), IPaymentRepository
{
    public Task<Payment?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken)
    {
        return DbSet.FirstOrDefaultAsync(p => p.EnrollmentId == enrollmentId, cancellationToken);
    }

    public Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken)
    {
        return DbSet.FirstOrDefaultAsync(p => p.PaymentIntentId == paymentIntentId, cancellationToken);
    }
}
