using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Common.Shared;
using Domain.Entities.PaymentAggregate;

namespace Domain.Entities.EnrollmentAggregate;

public class Enrollment : BaseAuditableEntity, IAggregateRoot
{
    private Enrollment() {}

    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    
    public DateTime? ExpiresAt { get; private set; }

    public static Result<Enrollment> Create(Guid userId, Guid courseId, string courseTitle, string? paidAmount)
    {
        if (userId == Guid.Empty)
            return EnrollmentErrors.UserIdRequired;
        
        if (courseId == Guid.Empty)
            return EnrollmentErrors.CourseIdRequired;

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = courseId,
            Status = EnrollmentStatus.Pending,
            ExpiresAt = null
        };

        enrollment.RaiseDomainEvent(new EnrollmentCreatedEvent(
            enrollment.Id,
            enrollment.UserId,
            enrollment.CourseId,
            courseTitle,
            paidAmount,
            enrollment.Status,
            DateTime.UtcNow,
            enrollment.ExpiresAt));
        
        return Result.Success(enrollment);
    }

    public void Activate()
    {
        if (Status == EnrollmentStatus.Confirmed)
            return;

        Status = EnrollmentStatus.Confirmed;
        RaiseDomainEvent(new EnrollmentActivatedEvent(Id, UserId, CourseId));
    }

    public Result MarkAsRefunded(Guid paymentId, string refundedAmount)
    {
        if (!IsEligibleForRefund())
            return PaymentErrors.NotEligibleForRefund;

        Status = EnrollmentStatus.Dropped;

        RaiseDomainEvent(new EnrollmentRefundedEvent(Id, paymentId, DateTime.UtcNow, refundedAmount, Status));
        
        return Result.Success(this);
    }

    public Result Fail()
    {
        if (Status != EnrollmentStatus.Confirmed)
            return EnrollmentErrors.AlreadyPaid;

        Status = EnrollmentStatus.PaymentFailed;
        RaiseDomainEvent(new EnrollmentRejectedEvent(UserId, CourseId, DateTime.UtcNow, Status));
        return Result.Success();
    }

    public void Enroll()
    {
        RaiseDomainEvent(new EnrollmentEnrolledEvent(Id, DateTime.UtcNow, Status));
    }

    public Result Drop()
    {
        if (Status == EnrollmentStatus.Confirmed)
            return EnrollmentErrors.CannotDropConfirmedEnrollment;

        Status = EnrollmentStatus.Dropped;
        RaiseDomainEvent(new EnrollmentDroppedEvent(Id, DateTime.UtcNow, Status));
        return Result.Success();
    }

    public bool IsEligibleForRefund() => Status == EnrollmentStatus.Confirmed;
}
