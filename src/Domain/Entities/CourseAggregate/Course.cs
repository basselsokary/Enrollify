using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Common.Shared;
using Domain.ValueObjects;

namespace Domain.Entities.CourseAggregate;

public class Course : BaseAuditableEntity, IAggregateRoot, ISoftDelete
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Money? Price { get; private set; }
    public int? Capacity { get; private set; }
    public bool? IsDeleted { get; set; }

    private Course() { }
    private Course(string title, string description, int? capacity, Money? price)
    {
        Title = title;
        Description = description;
        Price = price;
        Capacity = capacity;
    }

    public static Result<Course> Create(string title, string description, int? capacity, Money? price)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<Course>(CourseErrors.TitleRequired);

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure<Course>(CourseErrors.DescriptionRequired);

        if (capacity.HasValue && capacity <= 0)
            return Result.Failure<Course>(CourseErrors.CapacityMustBeGreaterThanZero);

        var course = new Course(title, description, capacity, price);
        course.RaiseDomainEvent(new CourseCreatedEvent(title, price));
        
        return Result.Success(course);
    }

    public void Delete()
    {
        IsDeleted = true;
        // RaiseDomainEvent(new CourseDeletedEvent(Title));
    }
    
}