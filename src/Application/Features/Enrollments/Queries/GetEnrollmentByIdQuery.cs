using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.ReadRepositories;
using Domain.Entities.EnrollmentAggregate;
using FluentValidation;

namespace Application.Features.Enrollments.Queries;

public sealed record GetEnrollmentByIdQuery(Guid EnrollmentId) : IQuery<UserEnrollmentResponse>;

internal sealed class GetEnrollmentByIdQueryHandler(
    IUserEnrollmentReadRepository readRepository,
    IUserContext userContext) : IQueryHandler<GetEnrollmentByIdQuery, UserEnrollmentResponse>
{
    public async Task<Result<UserEnrollmentResponse>> HandleAsync(GetEnrollmentByIdQuery query, CancellationToken cancellationToken)
    {
        var enrollment = await readRepository.GetEnrollmentByIdAsync(
            userContext.Id,
            query.EnrollmentId,
            cancellationToken);

        if (enrollment is null)
        {
            return EnrollmentErrors.EnrollmentNotFound;
        }

        return enrollment;
    }
}

public sealed record UserEnrollmentResponse(
    Guid Id,
    Guid UserId,
    Guid CourseId,
    string CourseTitle,
    EnrollmentStatus Status,
    DateTime EnrollmentAt,
    string? PaidAmount,
    DateTime? ExpiresAt);

internal sealed class GetEnrollmentByIdQueryValidator : AbstractValidator<GetEnrollmentByIdQuery>
{
    public GetEnrollmentByIdQueryValidator()
    {
        RuleFor(x => x.EnrollmentId)
            .NotEmpty()
            .WithMessage(EnrollmentErrors.EnrollmentIdRequired.Message);
    }
}
