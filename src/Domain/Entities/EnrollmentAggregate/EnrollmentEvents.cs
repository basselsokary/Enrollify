using Domain.Common;

namespace Domain.Entities.EnrollmentAggregate;

public class EnrollmentCreatedEvent(DateTime enrollmentAt, EnrollStatus status) : BaseEvent
{
    public DateTime EnrollmentAt { get; private set; } = enrollmentAt;
    public EnrollStatus Status { get; private set; } = status;
}