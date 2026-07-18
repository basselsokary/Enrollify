using Domain.Common.Shared;

namespace Domain.Entities.EnrollmentAggregate;

public class EnrollmentErrors
{
    public static Error EnrollmentIdRequired =>
        Error.Validation("Enrollment.Id.Required", "Enrollment ID cannot be empty.");
    public static Error EnrollmentNotFound =>
        Error.NotFound("Enrollment.NotFound", "Enrollment not found.");
    public static Error UserIdRequired =>
        Error.Validation("User.Id.Required", "User ID cannot be empty.");
    public static Error CourseIdRequired =>
        Error.Validation("Course.Id.Required", "Course ID cannot be empty.");
    public static Error AlreadyEnrolled =>
        Error.Validation("Enrollment.AlreadyEnrolled", "User is already enrolled in the course.");
    public static Error AlreadyPaid =>
        Error.Validation("Enrollment.AlreadyPaid", "Enrollment is already marked as paid.");
    public static Error CannotDropConfirmedEnrollment =>
        Error.Validation("Enrollment.CannotDropConfirmed", "Cannot drop an enrollment that is already confirmed.");
}