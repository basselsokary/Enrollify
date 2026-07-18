using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Infrastructure;
using Domain.Entities.CourseAggregate;
using Domain.Entities.EnrollmentAggregate;
using Domain.Entities.PaymentAggregate;
using Domain.Repositories;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Features.Enrollments.Commands;

public sealed record EnrollInCourseCommand(Guid CourseId) : ICommand<EnrollInCourseResponse>;

internal sealed class EnrollInCourseCommandHandler(
    IUnitOfWork unitOfWork,
    IPaymentService paymentService,
    IUserContext userContext) : ICommandHandler<EnrollInCourseCommand, EnrollInCourseResponse>
{
    public async Task<Result<EnrollInCourseResponse>> HandleAsync(EnrollInCourseCommand command, CancellationToken cancellationToken = default)
    {
        var course = await unitOfWork.Courses
            .GetByIdAsync(command.CourseId, cancellationToken);

        if (course is null)
            return CourseErrors.CourseNotFound;
        
        var isEnrolled = await unitOfWork.Enrollments.IsUserEnrolledAsync(
            command.CourseId,
            userContext.Id,
            cancellationToken);

        if (isEnrolled)
            return EnrollmentErrors.AlreadyEnrolled;

        var enrollmentResult = Enrollment.Create(userContext.Id, course.Id, course.Title, course.Price?.ToString());
        if (enrollmentResult.Failed)
            return enrollmentResult.Error;

        var enrollment = enrollmentResult.Value;
        await unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);

        string? clientSecret = null;
        if (!course.IsFree())
        {
            var paymentIntentResult = await paymentService.CreatePaymentIntentAsync(
                enrollment.Id,
                course.Price!,
                cancellationToken);

            if (paymentIntentResult.Failed)
            {
                enrollment.Fail();
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return paymentIntentResult.Error;
            }

            var paymentResult = Payment.Create(
                userContext.Id,
                enrollment.Id,
                paymentIntentResult.Value.PaymentIntentId,
                Money.Create(course.Price!.Amount, course.Price.Currency).Value,
                PaymentMethod.CreditCard,
                course.Title);
            
            if (paymentResult.Failed)
            {
                enrollment.Fail();
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return paymentResult.Error;
            }

            await unitOfWork.Payments.AddAsync(paymentResult.Value, cancellationToken);
            clientSecret = paymentIntentResult.Value.ClientSecret;
        }
        else
        {
            enrollment.Activate();
        }

        enrollment.Enroll();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new EnrollInCourseResponse(enrollment.Id, clientSecret);
    }
}

public sealed record EnrollInCourseResponse(Guid EnrollmentId, string? ClientSecret);

internal sealed class EnrollInCourseCommandValidator : AbstractValidator<EnrollInCourseCommand>
{
    public EnrollInCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage(CourseErrors.CourseIdRequired.Message);
    }
}