namespace Domain.Entities.PaymentAggregate;

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}
