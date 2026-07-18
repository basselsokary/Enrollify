using API.Controllers.Base;
using API.Dtos;
using Application.Common.Interfaces.Messaging;
using Application.Contracts.Common;
using Application.Features.Enrollments.Commands;
using Application.Features.Enrollments.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ApiBaseController
{
    [HttpGet("{enrollmentId:guid}")]
    public async Task<ActionResult<UserEnrollmentResponse>> GetByEnrollmentId(
        [FromRoute] Guid enrollmentId,
        [FromServices] IQueryHandler<GetEnrollmentByIdQuery, UserEnrollmentResponse> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetEnrollmentByIdQuery(enrollmentId);
        var result = await queryHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<UserEnrollmentsResponse>>> GetUserEnrollments(
        [FromQuery] PagingParametersRequest paging,
        [FromServices] IQueryHandler<GetEnrollmentsByUserIdQuery, PagedResult<UserEnrollmentsResponse>> queryHandler,
        CancellationToken cancellationToken = default)
    {
        var query = new GetEnrollmentsByUserIdQuery(new PagingParameters(paging.Page, paging.PageSize));
        var result = await queryHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("enroll")]
    public async Task<ActionResult<EnrollInCourseResponse>> EnrollInCourse(
        [FromBody] EnrollInCourseCommand command,
        [FromServices] ICommandHandler<EnrollInCourseCommand, EnrollInCourseResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("drop")]
    public async Task<ActionResult<Guid>> DropCourse(
        [FromBody] DropCourseCommand command,
        [FromServices] ICommandHandler<DropCourseCommand, Guid> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
