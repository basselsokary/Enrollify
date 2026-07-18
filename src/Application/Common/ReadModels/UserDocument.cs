namespace Application.Common.ReadModels;

public sealed class UserDocument
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string Status { get; set; }
    public bool IsActive { get; set; }
    public int TotalEnrollments { get; set; }

    public DateTime? LastLoginAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}