using Domain.Common.Shared;

namespace Domain.Entities.CourseAggregate;

public class CourseErrors
{
    public static Error CourseIdRequired =>
        Error.Validation("Course.Id.Required", "Course ID is required.");
    public static Error CourseNotFound =>
        Error.NotFound("Course.NotFound", "The specified course was not found.");
    public static Error TitleRequired =>
        Error.Validation("Course.Title.Required", "Course title cannot be empty.");
    public static Error DescriptionRequired =>
        Error.Validation("Course.Description.Required", "Course description cannot be empty.");
    public static Error PriceRequired =>
        Error.Validation("Course.Price.Required", "Course price must be greater than zero.");
    public static Error CapacityMustBeGreaterThanZero =>
        Error.Validation("Course.Capacity.MustBeGreaterThanZero", "Course capacity must be greater than zero.");
}