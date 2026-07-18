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
    public static Error DurationMustBeGreaterThanZero =>
        Error.Validation("Course.Duration.MustBeGreaterThanZero", "Course duration must be greater than zero.");
}