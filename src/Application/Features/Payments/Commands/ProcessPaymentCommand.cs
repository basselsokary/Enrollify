using Application.Common.Interfaces.Infrastructure;
using Domain.Entities.EnrollmentAggregate;
using Domain.Entities.PaymentAggregate;
using Domain.Repositories;
using FluentValidation;

namespace Application.Features.Payments.Commands;

public sealed record ProcessPaymentCommand(
    string Payload,
    string SignatureHeader) : ICommand<Guid>;

internal sealed class ProcessPaymentCommandHandler(
    IUnitOfWork unitOfWork,
    IPaymentService paymentService) : ICommandHandler<ProcessPaymentCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(ProcessPaymentCommand command, CancellationToken cancellationToken = default)
    {        
        var signatureResult = await paymentService.VerifySignatureAsync(
            command.Payload,
            command.SignatureHeader,
            cancellationToken);
        
        if (signatureResult.Failed)
            return signatureResult.Error;
        
        var enrollment = await unitOfWork.Enrollments.GetByIdAsync(
            signatureResult.Value.EnrollmentId, cancellationToken);
        
        if (enrollment is null)
            return EnrollmentErrors.EnrollmentNotFound;

        var payment = await unitOfWork.Payments
            .GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
        
        if (!signatureResult.Value.Verified)
        {
            var failResult = enrollment.Fail();
            if (failResult.Failed)
                return failResult.Error;

            failResult = payment?.Fail();
            if (failResult?.Failed ?? false)
                return failResult.Error;

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return PaymentErrors.InvalidSignature;
        }

        enrollment.Activate();
        payment?.Complete();
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(enrollment.Id);
    }
}

internal sealed class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.Payload)
            .NotEmpty();
        
        RuleFor(x => x.SignatureHeader)
            .NotEmpty();
    }
}