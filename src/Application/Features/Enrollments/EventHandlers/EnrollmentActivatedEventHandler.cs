using Application.Common.Interfaces.ReadRepositories;
using Domain.Common.Interfaces;
using Domain.Entities.EnrollmentAggregate;

namespace Application.Features.Enrollments.EventHandlers;

internal sealed class EnrollmentActivatedEventHandler(
    IUserEnrollmentReadRepository userEnrollmentReadRepository) : IDomainEventHandler<EnrollmentActivatedEvent>
{
    public async Task HandleAsync(EnrollmentActivatedEvent notification, CancellationToken cancellationToken = default)
    {
        await userEnrollmentReadRepository.UpdateStatusAsync(notification.EnrollmentId, EnrollmentStatus.Confirmed, cancellationToken);
    }
}