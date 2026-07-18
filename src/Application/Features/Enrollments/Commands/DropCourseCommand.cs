using Application.Common.Interfaces.Authentication;
using Domain.Entities.EnrollmentAggregate;
using Domain.Repositories;
using FluentValidation;

namespace Application.Features.Enrollments.Commands;

public sealed record DropCourseCommand(Guid EnrollmentId) : ICommand<Guid>;

internal sealed class DropCourseCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<DropCourseCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(DropCourseCommand command, CancellationToken cancellationToken = default)
    {
        var enrollment = await unitOfWork.Enrollments.GetByEnrollmentIdAndUserIdAsync(
            command.EnrollmentId, userContext.Id, cancellationToken);

        if (enrollment is null)
            return EnrollmentErrors.EnrollmentNotFound;

        var dropResult = enrollment.Drop();
        if (dropResult.Failed)
            return dropResult.Error;

        await unitOfWork.DeleteAsync(enrollment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(enrollment.Id);
    }
}

internal sealed class DropCourseCommandValidator : AbstractValidator<DropCourseCommand>
{
    public DropCourseCommandValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty()
            .WithMessage(EnrollmentErrors.EnrollmentIdRequired.Message);
    }
}