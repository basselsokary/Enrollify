using Application.Common.Interfaces.ReadRepositories;
using Application.Common.ReadModels;
using Domain.Common.Interfaces;
using Domain.Entities.EnrollmentAggregate;

namespace Application.Features.Enrollments.EventHandlers;

internal sealed class EnrollmentCreatedEventHandler(
    IUserEnrollmentReadRepository userEnrollmentReadRepository,
    ICourseReadRepository courseReadRepository) : IDomainEventHandler<EnrollmentCreatedEvent>
{
    public async Task HandleAsync(EnrollmentCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        var userEnrollmentDocument = new UserEnrollmentDocument
        {
            Id = notification.EnrollmentId,
            UserId = notification.UserId,
            CourseId = notification.CourseId,
            CourseTitle = notification.CourseTitle,
            Status = notification.Status,
            EnrollmentAt = notification.EnrollmentAt,
            PaidAmount = notification.PaidAmount,
            ExpiresAt = notification.ExpiresAt
        };

        await userEnrollmentReadRepository.AddAsync(userEnrollmentDocument, cancellationToken);
        await courseReadRepository.IncrementEnrollmentCountAsync(notification.CourseId, cancellationToken);
    }
}
