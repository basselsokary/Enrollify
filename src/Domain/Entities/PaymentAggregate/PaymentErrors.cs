using Domain.Common.Shared;

namespace Domain.Entities.PaymentAggregate;

public static class PaymentErrors
{
    public static Error PaymentIdRequired =>
        Error.Validation("Payment.Id.Required", "Payment ID is required.");
    public static Error PaymentNotFound =>
        Error.NotFound("Payment.Not.Found", "Payment not found.");
    public static Error PaymentAlreadyProcessed =>
        Error.Validation("Payment.Already.Processed", "Payment has already been processed.");
    public static Error NotEligibleForPayment =>
        Error.Validation("Payment.Not.Eligible", "Payment is not eligible for processing.");
    public static Error NotEligibleForRefund =>
        Error.Validation("Payment.Not.Eligible.For.Refund", "Payment is not eligible for refund.");
}
