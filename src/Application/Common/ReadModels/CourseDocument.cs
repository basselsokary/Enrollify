namespace Application.Common.ReadModels;

public sealed class CourseDocument
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required bool IsFree { get; set; }
    public required int DurationInMinutes { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int EnrolledCount { get; set; }
    
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
