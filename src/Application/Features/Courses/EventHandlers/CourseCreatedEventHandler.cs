using Application.Common.Interfaces.ReadRepositories;
using Application.Common.ReadModels;
using Domain.Common.Interfaces;
using Domain.Entities.CourseAggregate;

namespace Application.Features.Courses.EventHandlers;

public sealed class CourseCreatedEventHandler(
    ICourseReadRepository courseReadRepository) : IDomainEventHandler<CourseCreatedEvent>
{
    public async Task HandleAsync(CourseCreatedEvent notification, CancellationToken cancellationToken)
    {
        var courseDocument = new CourseDocument
        {
            Id = notification.Id,
            Title = notification.Title,
            Description = notification.Description,
            Price = notification.PriceAmount,
            Currency = notification.PriceCurrency,
            IsFree = notification.PriceAmount == null,
            DurationInMinutes = notification.DurationInMinutes,
            CreatedAt = notification.CreatedAt,
            LastModifiedAt = notification.LastModifiedAt
        };

        await courseReadRepository.AddAsync(courseDocument, cancellationToken);
    }
}