using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.Features.Payments.Commands;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("webhooks")]
public class StripeWebhookController : ApiBaseController
{
    [HttpPost("stripe")]
    public async Task<ActionResult<Guid>> HandleStripeWebhook(
        [FromServices] ICommandHandler<ProcessPaymentCommand, Guid> commandHandler)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"];

        var command = new ProcessPaymentCommand(json, signatureHeader!);
        var result = await commandHandler.HandleAsync(command);
        return HandleResult(result);
    }
}


