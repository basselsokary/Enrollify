namespace Domain.Entities.PaymentAggregate;

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Dropped,
    Refunded
}

public enum PaymentMethod
{
    CreditCard
}
