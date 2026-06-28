using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Common.Shared;
using Domain.Entities.PaymentAggregate;

namespace Domain.Entities.EnrollmentAggregate;

public class Enrollment : BaseAuditableEntity, IAggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public EnrollStatus Status { get; private set; }
    public DateTime EnrollmentAt { get; private set; }
    
    public DateTime? PaidAt { get; private set; }

    private Enrollment() {}
    private Enrollment(Guid userId, Guid courseId)
    {
        UserId = userId;
        CourseId = courseId;
        EnrollmentAt = DateTime.UtcNow;
        Status = EnrollStatus.Pending;
        PaidAt = null;
    }

    public static Result<Enrollment> Create(Guid userId, Guid courseId)
    {
        if (userId == Guid.Empty)
            return EnrollmentErrors.UserIdRequired;
        
        if (courseId == Guid.Empty)
            return EnrollmentErrors.CourseIdRequired;

        var enrollment = new Enrollment(userId, courseId);
        enrollment.RaiseDomainEvent(new EnrollmentCreatedEvent(enrollment.EnrollmentAt, enrollment.Status));
        
        return Result.Success(enrollment);
    }

    public Result<Enrollment> MarkAsPaid()
    {
        if (!IsEligibleForPayment())
            return EnrollmentErrors.AlreadyPaid;

        Status = EnrollStatus.Confirmed;
        PaidAt = DateTime.UtcNow;

        // RaiseDomainEvent(new EnrollmentPaidEvent(PaidAt, paymentId));
        
        return Result.Success(this);
    }

    public Result<Enrollment> MarkAsRefunded()
    {
        if (!IsEligibleForRefund())
            return PaymentErrors.NotEligibleForRefund;

        Status = EnrollStatus.Cancelled;
        PaidAt = null;

        // RaiseDomainEvent(new EnrollmentRefundedEvent(DateTime.UtcNow, paymentId));
        
        return Result.Success(this);
    }

    public bool IsEligibleForPayment() => Status == EnrollStatus.Pending;

    public bool IsEligibleForRefund() => Status == EnrollStatus.Confirmed;
}
