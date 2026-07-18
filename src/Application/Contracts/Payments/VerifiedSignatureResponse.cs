namespace Application.Contracts.Payments;

public sealed record VerifiedSignatureResponse(
    Guid EnrollmentId,
    string PaymentIntentId,
    bool Verified = true);
