using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Infrastructure;
using Domain.Entities.EnrollmentAggregate;
using Domain.Entities.PaymentAggregate;
using Domain.Repositories;
using FluentValidation;

namespace Application.Features.Payments.Commands;

public sealed record RefundEnrollmentCommand(Guid EnrollmentId, Guid PaymentId) : ICommand<Guid>;

internal sealed class RefundEnrollmentCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext,
    IPaymentService paymentService) : ICommandHandler<RefundEnrollmentCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(RefundEnrollmentCommand command, CancellationToken cancellationToken = default)
    {
        var enrollment = await unitOfWork.Enrollments.GetByEnrollmentIdAndUserIdAsync(
            command.EnrollmentId, userContext.Id, cancellationToken);

        if (enrollment is null)
            return EnrollmentErrors.EnrollmentNotFound;

        if (!enrollment.IsEligibleForRefund())
            return PaymentErrors.NotEligibleForRefund;
        
        var payment = await unitOfWork.Payments.GetByIdAsync(command.PaymentId, cancellationToken);
        if (payment is null)
            return PaymentErrors.PaymentNotFound;
        
        if (!payment.IsEligibleForRefund())
            return PaymentErrors.NotEligibleForRefund;

        var result = payment.Refund();
        if (result.Failed)
            return result.Error;
        
        result = enrollment.MarkAsRefunded(payment.Id, payment.Money.ToString());
        if (result.Failed)
            return result.Error;
        
        var refundResult = await paymentService.RefundPaymentAsync(payment.PaymentIntentId, null, cancellationToken); 
        if (refundResult.Failed)
            return refundResult.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(payment.Id);
    }
}

internal sealed class RefundEnrollmentCommandValidator : AbstractValidator<RefundEnrollmentCommand>
{
    public RefundEnrollmentCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty()
            .WithMessage(EnrollmentErrors.EnrollmentIdRequired.Message);
        
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage(PaymentErrors.PaymentIdRequired.Message);
    }
}