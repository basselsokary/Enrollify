using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Common.Shared;
using Domain.ValueObjects;

namespace Domain.Entities.PaymentAggregate;

public class Payment : BaseAuditableEntity, IAggregateRoot
{
    public Money Amount { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? PaymentIntentId { get; private set; }
    public Guid EnrollmentId { get; private set; }

    private Payment(Guid enrollmentId, Money money, PaymentMethod paymentMethod)
    {
        Amount = money;
        PaymentMethod = paymentMethod;
        Status = PaymentStatus.Pending;
        EnrollmentId = enrollmentId;
    }

    public static Result<Payment> Create(Guid enrollmentId, Money money, PaymentMethod paymentMethod)
    {
        var payment = new Payment(enrollmentId, money, paymentMethod);
        return Result.Success(payment);
    }

    public Result Complete()
    {
        if (Status != PaymentStatus.Pending)
            return Result.Failure(PaymentErrors.PaymentAlreadyProcessed);

        Status = PaymentStatus.Completed;
        return Result.Success();
    }

    public Result Fail()
    {
        if (Status != PaymentStatus.Pending)
            return Result.Failure(PaymentErrors.PaymentAlreadyProcessed);

        Status = PaymentStatus.Failed;
        return Result.Success();
    }

    public Result Refund()
    {
        if (Status != PaymentStatus.Completed)
            return Result.Failure(PaymentErrors.NotEligibleForRefund);

        Status = PaymentStatus.Refunded;
        return Result.Success();
    }
}
