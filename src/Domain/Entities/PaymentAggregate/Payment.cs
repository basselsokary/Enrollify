using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Common.Shared;
using Domain.ValueObjects;

namespace Domain.Entities.PaymentAggregate;

public class Payment : BaseAuditableEntity, IAggregateRoot
{
    public Guid UserId { get; set; }
    public Guid EnrollmentId { get; private set; }
    public string PaymentIntentId { get; private set; } = null!;
    
    public Money Money { get; private set; } = null!;
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus Status { get; private set; }

    public decimal? AmountRefunded { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private Payment() { }

    public static Result<Payment> Create(
        Guid userId,
        Guid enrollmentId,
        string paymentIntentId,
        Money money,
        PaymentMethod paymentMethod,
        string courseTitle)
    {
        var payment = new Payment
        {
            UserId = userId,
            EnrollmentId = enrollmentId,
            PaymentIntentId = paymentIntentId,
            Money = money,
            PaymentMethod = paymentMethod,
            Status = PaymentStatus.Pending,
        };

        payment.RaiseDomainEvent(new PaymentCreatedEvent(
            payment.Id,
            userId,
            enrollmentId,
            paymentIntentId,
            courseTitle,
            money.Amount,
            money.Currency,
            paymentMethod,
            payment.Status,
            DateTime.UtcNow));

        return Result.Success(payment);
    }

    public void Complete()
    {
        if (Status == PaymentStatus.Completed)
            return; // Payment is already completed, no action needed.

        Status = PaymentStatus.Completed;
        PaidAt = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentCompletedEvent(UserId, EnrollmentId, PaidAt.Value, Status));
    }

    public Result Fail()
    {
        if (Status != PaymentStatus.Pending)
            return Result.Failure(PaymentErrors.PaymentAlreadyProcessed);

        Status = PaymentStatus.Failed;

        RaiseDomainEvent(new PaymentFailedEvent(UserId, EnrollmentId, DateTime.UtcNow, Status));
        return Result.Success();
    }

    public Result Refund()
    {
        if (AmountRefunded.HasValue)
            return Result.Failure(PaymentErrors.AlreadyRefunded);
        
        if (Status == PaymentStatus.Failed)
            return Result.Failure(PaymentErrors.NotEligibleForRefund);

        AmountRefunded = Money.Amount;
        Status = PaymentStatus.Refunded;

        RaiseDomainEvent(new PaymentRefundedEvent(UserId, EnrollmentId, DateTime.UtcNow, Status));
        return Result.Success();
    }

    public bool IsEligibleForRefund()
    {
        return Status == PaymentStatus.Completed && !AmountRefunded.HasValue;
    }
}
