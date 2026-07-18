using Application.Common.Interfaces.ReadRepositories;
using Domain.Common.Interfaces;
using Domain.Entities.EnrollmentAggregate;

namespace Application.Features.Payments.EventHandlers;

internal sealed class EnrollmentRefundedEventHandler(
    IUserEnrollmentReadRepository userEnrollmentReadRepository,
    IPaymentReadRepository paymentReadRepository) : IDomainEventHandler<EnrollmentRefundedEvent>
{
    public async Task HandleAsync(EnrollmentRefundedEvent notification, CancellationToken cancellationToken = default)
    {
        var existingEnrollment = await userEnrollmentReadRepository.GetByIdAsync(
            notification.EnrollmentId,
            cancellationToken);
        
        if (existingEnrollment is null)
            return;
        
        existingEnrollment.Status = notification.Status;
        existingEnrollment.PaidAmount = notification.RefundedAmount;
        await userEnrollmentReadRepository.UpdateAsync(existingEnrollment, cancellationToken);
        
        var existingPayment = await paymentReadRepository.GetByIdAsync(
            notification.PaymentId,
            cancellationToken);
        
        if (existingPayment is null)
            return;

    
        existingPayment.AmountRefunded = notification.RefundedAmount;
        await paymentReadRepository.UpdateAsync(existingPayment, cancellationToken);
    }
}
