using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities.CourseAggregate;

public class CourseCreatedEvent(string title, Money? price) : BaseEvent
{
    public string Title { get; private set; } = title;
    public Money? Price { get; private set; } = price;
}