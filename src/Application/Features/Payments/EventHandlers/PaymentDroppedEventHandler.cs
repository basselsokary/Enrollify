using Application.Common.Interfaces.ReadRepositories;
using Domain.Common.Interfaces;
using Domain.Entities.PaymentAggregate;

namespace Application.Features.Payments.EventHandlers;

internal sealed class PaymentDroppedEventHandler(
    IPaymentReadRepository readRepository) : IDomainEventHandler<PaymentDroppedEvent>
{
    public async Task HandleAsync(PaymentDroppedEvent notification, CancellationToken cancellationToken = default)
    {
        var paymentDocument = await readRepository.GetByIdAsync(notification.PaymentId, cancellationToken);
        if (paymentDocument is null)
            return;

        paymentDocument.Status = notification.Status;

        await readRepository.UpdateAsync(paymentDocument, cancellationToken);
    }
}
