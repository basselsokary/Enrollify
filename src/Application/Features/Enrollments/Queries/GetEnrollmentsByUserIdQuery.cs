using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.ReadRepositories;
using Application.Contracts.Common;
using Application.Features.Common.Validators;
using Domain.Entities.EnrollmentAggregate;
using FluentValidation;

namespace Application.Features.Enrollments.Queries;

public sealed record GetEnrollmentsByUserIdQuery(
    PagingParameters Paging) : IQuery<PagedResult<UserEnrollmentsResponse>>;

internal sealed class GetEnrollmentsByUserIdQueryHandler(
    IUserEnrollmentReadRepository readRepository,
    IUserContext userContext) : IQueryHandler<GetEnrollmentsByUserIdQuery, PagedResult<UserEnrollmentsResponse>>
{
    public async Task<Result<PagedResult<UserEnrollmentsResponse>>> HandleAsync(GetEnrollmentsByUserIdQuery query, CancellationToken cancellationToken)
    {
        var enrollments = await readRepository.GetEnrollmentsByUserIdAsync(
            userContext.Id,
            query.Paging,
            cancellationToken);
        
        return Result.Success(enrollments);
    }
}

public sealed record UserEnrollmentsResponse(
    Guid Id,
    Guid UserId,
    Guid CourseId,
    string CourseTitle,
    EnrollmentStatus Status,
    DateTime EnrollmentAt,
    string? PaidAmount,
    DateTime? ExpiresAt);

internal sealed class GetEnrollmentsByUserIdQueryValidator : AbstractValidator<GetEnrollmentsByUserIdQuery>
{
    public GetEnrollmentsByUserIdQueryValidator()
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());
    }
}
