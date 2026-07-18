using Domain.Common;

namespace Domain.Entities.PaymentAggregate;

public sealed class PaymentCreatedEvent(
    Guid paymentId,
    Guid userId,
    Guid enrollmentId,
    string paymentIntentId,
    string courseTitle,
    decimal paidAmount,
    string currency,
    PaymentMethod paymentMethod,
    PaymentStatus status,
    DateTime createdAt) : BaseEvent
{
    public Guid PaymentId { get; } = paymentId;
    public Guid UserId { get; } = userId;
    public Guid EnrollmentId { get; } = enrollmentId;
    public string PaymentIntentId { get; } = paymentIntentId;
    public string CourseTitle { get; } = courseTitle;
    public decimal PaidAmount { get; } = paidAmount;
    public string Currency { get; } = currency;
    public PaymentMethod PaymentMethod { get; } = paymentMethod;
    public PaymentStatus Status { get; } = status;
    public DateTime CreatedAt { get; } = createdAt;
}

public sealed class PaymentCompletedEvent(
    Guid paymentId, DateTime paidAt, PaymentStatus status) : BaseEvent
{
    public Guid PaymentId { get; } = paymentId;
    public DateTime PaidAt { get; } = paidAt;
    public PaymentStatus Status { get; } = status;
}

public sealed class PaymentFailedEvent(
    Guid userId, Guid enrollmentId, DateTime failedAt, PaymentStatus status) : BaseEvent
{
    public Guid UserId { get; } = userId;
    public Guid EnrollmentId { get; } = enrollmentId;
    public DateTime FailedAt { get; } = failedAt;
    public PaymentStatus Status { get; } = status;
}

public sealed class PaymentDroppedEvent(
    Guid paymentId, Guid enrollmentId, DateTime droppedAt, PaymentStatus status) : BaseEvent
{
    public Guid PaymentId { get; } = paymentId;
    public Guid EnrollmentId { get; } = enrollmentId;
    public DateTime DroppedAt { get; } = droppedAt;
    public PaymentStatus Status { get; } = status;
}

public sealed class PaymentRefundedEvent(
    Guid userId, Guid enrollmentId, DateTime refundedAt, PaymentStatus status) : BaseEvent
{
    public Guid UserId { get; } = userId;
    public Guid EnrollmentId { get; } = enrollmentId;
    public DateTime RefundedAt { get; } = refundedAt;
    public PaymentStatus Status { get; } = status;
}