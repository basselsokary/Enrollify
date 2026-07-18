using Domain.Entities.PaymentAggregate;

namespace Application.Common.ReadModels;

public sealed class PaymentDocument
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid EnrollmentId { get; set; }
    public required string PaymentIntentId { get; set; }
    public required string CourseTitle { get; set; }
    public required decimal PaidAmount { get; set; }
    public required string Currency { get; set; }
    public required PaymentStatus Status { get; set; }
    public required PaymentMethod PaymentMethod { get; set; }
    
    public string? AmountRefunded { get; set; }
    public DateTime? PaidAt { get; set; }
    public required DateTime CreatedAt { get; set; }
}