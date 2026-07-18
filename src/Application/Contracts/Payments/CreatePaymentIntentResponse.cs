namespace Application.Contracts.Payments;

public sealed record CreatePaymentIntentResponse(
    string PaymentIntentId,
    string ClientSecret);