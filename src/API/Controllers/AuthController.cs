using API.Configurations;
using API.Controllers.Base;
using Application.Common.Interfaces.Messaging;
using Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers;

[Route("api/[controller]")]
[EnableRateLimiting(RateLimiterPolicies.AuthPerIp)]
public class AuthController : ApiBaseController
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginCommand command,
        [FromServices] ICommandHandler<LoginCommand, LoginResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        [FromServices] ICommandHandler<RegisterCommand> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshResponse>> Refresh(
        [FromBody] RefreshCommand command,
        [FromServices] ICommandHandler<RefreshCommand, RefreshResponse> commandHandler,
        CancellationToken cancellationToken = default)
    {
        var result = await commandHandler.HandleAsync(command, cancellationToken);
        
        return HandleResult(result);
    }
}
