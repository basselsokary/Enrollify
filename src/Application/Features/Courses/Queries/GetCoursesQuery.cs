using Application.Common.Interfaces.ReadRepositories;
using Application.Contracts.Common;
using Application.Features.Common.Validators;
using FluentValidation;

namespace Application.Features.Courses.Queries;

public sealed record GetCoursesQuery(
    PagingParameters Paging) : IQuery<PagedResult<GetCoursesResponse>>;

internal sealed class GetCoursesQueryHandler(
    ICourseReadRepository readRepository) : IQueryHandler<GetCoursesQuery, PagedResult<GetCoursesResponse>>
{
    public async Task<Result<PagedResult<GetCoursesResponse>>> HandleAsync(GetCoursesQuery query, CancellationToken cancellationToken)
    {
        var courses = await readRepository.GetCoursesAsync(query.Paging, cancellationToken);

        return Result.Success(courses);
    }
}

public sealed record GetCoursesResponse(
    Guid Id,
    string Title,
    string Description,
    decimal? Price,
    string? Currency,
    bool IsFree,
    int DurationInMinutes,
    int EnrolledCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

internal sealed class GetCoursesQueryValidator : AbstractValidator<GetCoursesQuery>
{
    public GetCoursesQueryValidator()
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());
    }
}