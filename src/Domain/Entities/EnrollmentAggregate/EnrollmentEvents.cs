using Domain.Common;

namespace Domain.Entities.EnrollmentAggregate;

public sealed class EnrollmentCreatedEvent(
    Guid enrollmentId,
    Guid userId,
    Guid courseId,
    string courseTitle,
    string? paidAmount,
    EnrollmentStatus status,
    DateTime enrollmentAt,
    DateTime? expiresAt) : BaseEvent
{
    public Guid EnrollmentId { get; } = enrollmentId;
    public Guid UserId { get; } = userId;
    public Guid CourseId { get; } = courseId;
    public string CourseTitle { get; } = courseTitle;
    public string? PaidAmount { get; } = paidAmount;
    public EnrollmentStatus Status { get; } = status;
    public DateTime EnrollmentAt { get; } = enrollmentAt;
    public DateTime? ExpiresAt { get; } = expiresAt;
}

public sealed class EnrollmentActivatedEvent(Guid enrollmentId, Guid userId, Guid CourseId) : BaseEvent
{
    public Guid EnrollmentId { get; } = enrollmentId;
    public Guid UserId { get; } = userId;
    public Guid CourseId { get; } = CourseId;
}

public sealed class EnrollmentRefundedEvent(
    Guid enrollmentId, Guid paymentId, DateTime refundedAt, string refundedAmount, EnrollmentStatus status) : BaseEvent
{
    public Guid EnrollmentId { get; } = enrollmentId;
    public Guid PaymentId { get; } = paymentId;
    public string RefundedAmount { get; } = refundedAmount;
    public DateTime RefundedAt { get; } = refundedAt;
    public EnrollmentStatus Status { get; } = status;
}

public sealed class EnrollmentDroppedEvent(Guid enrollmentId, DateTime droppedAt, EnrollmentStatus status) : BaseEvent
{
    public Guid EnrollmentId { get; } = enrollmentId;
    public DateTime DroppedAt { get; } = droppedAt;
    public EnrollmentStatus Status { get; } = status;
}

public sealed class EnrollmentRejectedEvent(
    Guid userId, Guid courseId, DateTime rejectedAt, EnrollmentStatus status) : BaseEvent
{
    public Guid UserId { get; } = userId;
    public Guid CourseId { get; } = courseId;
    public DateTime RejectedAt { get; } = rejectedAt;
    public EnrollmentStatus Status { get; } = status;
}

public sealed class EnrollmentEnrolledEvent(
    Guid enrollmentId, DateTime enrolledAt, EnrollmentStatus status) : BaseEvent
{
    public Guid EnrollmentId { get; } = enrollmentId;
    public DateTime EnrolledAt { get; } = enrolledAt;
    public EnrollmentStatus Status { get; } = status;
}