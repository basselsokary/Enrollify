using Domain.Entities.EnrollmentAggregate;

namespace Application.Common.ReadModels;

public sealed class UserEnrollmentDocument
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid CourseId { get; set; }
    public required string CourseTitle { get; set; }
    public required EnrollmentStatus Status { get; set; }
    public required DateTime EnrollmentAt { get; set; }
    
    public string? PaidAmount { get; set; }
    public DateTime? ExpiresAt { get; set; }
}