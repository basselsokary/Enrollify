using Domain.Common.Shared;

namespace Domain.Entities.PaymentAggregate;

public static class PaymentErrors
{
    public static Error PaymentIdRequired =>
        Error.Validation("Payment.IdRequired", "Payment ID is required.");
    public static Error PaymentNotFound =>
        Error.NotFound("Payment.NotFound", "Payment not found.");
    public static Error PaymentAlreadyProcessed =>
        Error.Validation("Payment.AlreadyProcessed", "Payment has already been processed.");
    public static Error NotEligibleForRefund =>
        Error.Validation("Payment.NotEligibleForRefund", "Payment is not eligible for refund.");
    public static Error AlreadyRefunded =>
        Error.Validation("Payment.AlreadyRefunded", "Payment has already been refunded.");
    public static Error UnhandledEventType =>
        Error.Validation("Payment.UnhandledEventType", "Unhandled event type received from payment service.");
    public static Error InvalidSignature =>
        Error.Validation("Payment.InvalidSignature", "Invalid signature received from payment service.");
}
