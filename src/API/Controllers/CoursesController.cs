using API.Controllers.Base;
using API.Dtos;
using Application.Common.Interfaces.Messaging;
using Application.Contracts.Common;
using Application.Features.Courses.Queries;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
public class CoursesController : ApiBaseController
{
    [HttpGet("{courseId:guid}")]
    public async Task<ActionResult<GetCourseByIdResponse>> GetCourseById(
        [FromRoute] Guid courseId,
        [FromServices] IQueryHandler<GetCourseByIdQuery, GetCourseByIdResponse> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCourseByIdQuery(courseId);
        var result = await queryHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<GetCoursesResponse>>> GetCourses(
        [FromQuery] PagingParametersRequest paging,
        [FromServices] IQueryHandler<GetCoursesQuery, PagedResult<GetCoursesResponse>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCoursesQuery(new PagingParameters(paging.Page, paging.PageSize));
        var result = await queryHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
