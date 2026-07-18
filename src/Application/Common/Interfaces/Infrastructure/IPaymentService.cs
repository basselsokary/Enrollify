using Application.Contracts.Payments;
using Domain.ValueObjects;

namespace Application.Common.Interfaces.Infrastructure;

public interface IPaymentService
{
    Task<Result<CreatePaymentIntentResponse>> CreatePaymentIntentAsync(
        Guid enrollmentId,
        Money money,
        CancellationToken cancellationToken = default);
    Task<Result<string>> RefundPaymentAsync(
        string paymentIntentId,
        decimal? amount,
        CancellationToken cancellationToken = default);
    Task<Result<VerifiedSignatureResponse>> VerifySignatureAsync(
        string payload,
        string signatureHeader,
        CancellationToken cancellationToken = default);
}
