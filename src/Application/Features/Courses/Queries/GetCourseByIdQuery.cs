using Application.Common.Interfaces.ReadRepositories;
using Domain.Entities.CourseAggregate;
using FluentValidation;

namespace Application.Features.Courses.Queries;

public sealed record GetCourseByIdQuery(
    Guid CourseId) : IQuery<GetCourseByIdResponse>;

internal sealed class GetCourseByIdQueryHandler(
    ICourseReadRepository readRepository) : IQueryHandler<GetCourseByIdQuery, GetCourseByIdResponse>
{
    public async Task<Result<GetCourseByIdResponse>> HandleAsync(GetCourseByIdQuery query, CancellationToken cancellationToken)
    {
        var course = await readRepository.GetCourseByIdAsync(query.CourseId, cancellationToken);
        if (course is null)
        {
            return CourseErrors.CourseNotFound;
        }

        return Result.Success(course);
    }
}

public sealed record GetCourseByIdResponse(
    Guid Id,
    string Title,
    string Description,
    int DurationInMinutes,
    decimal? Price,
    string? Currency,
    bool IsFree,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

internal sealed class GetCourseByIdQueryValidator : AbstractValidator<GetCourseByIdQuery>
{
    public GetCourseByIdQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage(CourseErrors.CourseIdRequired.Message);
    }
}