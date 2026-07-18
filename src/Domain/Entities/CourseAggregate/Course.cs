using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Common.Shared;
using Domain.ValueObjects;

namespace Domain.Entities.CourseAggregate;

public class Course : BaseAuditableEntity, IAggregateRoot, ISoftDelete
{
    private Course() { }

    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int DurationInMinutes { get; private set; }
    public CourseType Type { get; private set; }
    public Money? Price { get; private set; }
    public bool IsDeleted { get; set; }

    public static Result<Course> Create(
        string title,
        string description,
        int durationInMinutes,
        CourseType type,
        Money? price)
    {
        if (string.IsNullOrWhiteSpace(title))
            return CourseErrors.TitleRequired;

        if (string.IsNullOrWhiteSpace(description))
            return CourseErrors.DescriptionRequired;

        if (durationInMinutes <= 0)
            return CourseErrors.DurationMustBeGreaterThanZero;

        var course = new Course
        {
            Title = title,
            Description = description,
            DurationInMinutes = durationInMinutes,
            Type = type,
            Price = price,
            IsDeleted = false
        };

        course.RaiseDomainEvent(new CourseCreatedEvent(
            course.Id,
            course.Title,
            course.Description,
            course.Price?.Amount,
            course.Price?.Currency,
            course.DurationInMinutes,
            DateTime.UtcNow,
            course.LastModifiedAt));
        
        return Result.Success(course);
    }

    public bool IsFree()
    {
        return Price is null || Price.Amount <= 0;
    }
}