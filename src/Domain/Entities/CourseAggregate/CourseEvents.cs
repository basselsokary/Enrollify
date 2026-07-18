using Domain.Common;

namespace Domain.Entities.CourseAggregate;

public sealed class CourseCreatedEvent(
    Guid id,
    string title,
    string description,
    decimal? priceAmount,
    string? priceCurrency,
    int durationInMinutes,
    DateTime CreatedAt,
    DateTime? LastModifiedAt) : BaseEvent
{
    public Guid Id { get; init; } = id;
    public string Title { get; init; } = title;
    public string Description { get; init; } = description;
    public decimal? PriceAmount { get; init; } = priceAmount;
    public string? PriceCurrency { get; init; } = priceCurrency;
    public int DurationInMinutes { get; init; } = durationInMinutes;
    public DateTime CreatedAt { get; init; } = CreatedAt;
    public DateTime? LastModifiedAt { get; init; } = LastModifiedAt;
}
