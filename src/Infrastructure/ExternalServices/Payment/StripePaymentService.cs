using Application.Common.Interfaces.Infrastructure;
using Application.Contracts.Payments;
using Domain.Common.Shared;
using Domain.Entities.PaymentAggregate;
using Domain.ValueObjects;
using Microsoft.Extensions.Options;
using Stripe;

namespace Infrastructure.ExternalServices.Payment;

internal class StripePaymentService(IOptions<StripeOptions> stripeOptions) : IPaymentService
{
    private readonly StripeOptions _stripeOptions = stripeOptions.Value;

    public async Task<Result<CreatePaymentIntentResponse>> CreatePaymentIntentAsync(
        Guid enrollmentId,
        Money money,
        CancellationToken cancellationToken = default)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(money.Amount * 100), // convert to cents
            Currency = money.Currency,
            Metadata = new Dictionary<string, string>
            {
                { "enrollment_id", enrollmentId.ToString() }
            }
        };

        var requestOptions = new RequestOptions
        {
            IdempotencyKey = enrollmentId.ToString() // prevents double charges
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options, requestOptions, cancellationToken);
        
        return new CreatePaymentIntentResponse(intent.Id, intent.ClientSecret);
    }

    public async Task<Result<string>> RefundPaymentAsync(
        string paymentIntentId,
        decimal? amount,
        CancellationToken cancellationToken = default)
    {
        var options = new RefundCreateOptions
        {
            PaymentIntent = paymentIntentId,
            Amount = amount.HasValue ? (long)(amount * 100) : null // convert to cents if amount is provided
        };

        var service = new RefundService();
        var refund = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return refund.Id;
    }

    public async Task<Result<VerifiedSignatureResponse>> VerifySignatureAsync(
        string payload, string signatureHeader, CancellationToken cancellationToken = default)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload, signatureHeader, _stripeOptions.WebhookSecret);

            return stripeEvent.Type switch
            {
                EventTypes.PaymentIntentSucceeded =>
                    Result.Success(MapToPaymentDto(stripeEvent) with { Verified = true }),

                EventTypes.PaymentIntentPaymentFailed =>
                    Result.Success(MapToPaymentDto(stripeEvent) with { Verified = false }),

                _ => PaymentErrors.UnhandledEventType // charge.* and others ignored, no cast attempted
            };
        }
        catch (StripeException)
        {
            return PaymentErrors.InvalidSignature;
        }
    }

    private static VerifiedSignatureResponse MapToPaymentDto(Event stripeEvent)
    {
        var intent = stripeEvent.Data.Object as PaymentIntent
            ?? throw new InvalidOperationException("PaymentIntent data is missing in the event.");
        
        var enrollmentId = Guid.TryParse(intent.Metadata["enrollment_id"], out var id) ? id : Guid.Empty;
        if (enrollmentId == Guid.Empty)
            throw new InvalidOperationException("Enrollment ID is missing or invalid in the PaymentIntent metadata.");

        return new VerifiedSignatureResponse(enrollmentId, intent.Id);
    }
}
