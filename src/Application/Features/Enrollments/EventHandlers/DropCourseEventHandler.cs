using Application.Common.Interfaces.ReadRepositories;
using Domain.Common.Interfaces;
using Domain.Entities.EnrollmentAggregate;

namespace Application.Features.Enrollments.EventHandlers;

internal sealed class DropCourseEventHandler(
    IUserEnrollmentReadRepository readRepository) : IDomainEventHandler<EnrollmentDroppedEvent>
{
    public async Task HandleAsync(EnrollmentDroppedEvent notification, CancellationToken cancellationToken = default)
    {
        await readRepository.DeleteAsync(notification.EnrollmentId, cancellationToken);
    }
}
