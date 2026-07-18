using Application.Common.Interfaces.ReadRepositories;
using Application.Common.ReadModels;
using Domain.Common.Interfaces;
using Domain.Entities.PaymentAggregate;

namespace Application.Features.Payments.EventHandlers;

internal sealed class PaymentCreatedEventHandler(
    IPaymentReadRepository readRepository) : IDomainEventHandler<PaymentCreatedEvent>
{
    public async Task HandleAsync(PaymentCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        var paymentDocument = new PaymentDocument
        {
            Id = notification.PaymentId,
            UserId = notification.UserId,
            EnrollmentId = notification.EnrollmentId,
            PaymentIntentId = notification.PaymentIntentId,
            CourseTitle = notification.CourseTitle,
            PaidAmount = notification.PaidAmount,
            Currency = notification.Currency,
            Status = notification.Status,
            PaymentMethod = notification.PaymentMethod,
            CreatedAt = notification.CreatedAt
        };

        await readRepository.AddAsync(paymentDocument, cancellationToken);
    }
}
