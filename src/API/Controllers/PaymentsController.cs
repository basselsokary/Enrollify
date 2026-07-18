using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.Contracts.Common;
using Application.Features.Payments.Commands;
using Application.Features.Payments.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ApiBaseController
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserPaymentResponse>>> GetPayments(
        [FromServices] IQueryHandler<GetPaymentsByUserIdQuery, PagedResult<UserPaymentResponse>> queryHandler,
        [FromQuery] PagingParameters paging,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPaymentsByUserIdQuery(paging);
        var result = await queryHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("refund")]
    public async Task<ActionResult<Guid>> RefundEnrollment(
        [FromServices] ICommandHandler<RefundEnrollmentCommand, Guid> commandHandler,
        [FromBody] RefundEnrollmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
