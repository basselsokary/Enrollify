using Domain.Entities.PaymentAggregate;

namespace Domain.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<Payment?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken);
    Task<Payment?> GetByPaymentIntentIdAsync(string paymentIntentId, CancellationToken cancellationToken);
}
