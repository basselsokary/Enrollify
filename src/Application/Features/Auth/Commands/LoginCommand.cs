using Application.Common.Interfaces.Authentication;
using Domain.Common;
using Domain.Common.Constants;
using FluentValidation;

namespace Application.Features.Auth.Commands;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<LoginResponse>;

public sealed class LoginCommandHandler(
    IIdentityService identityService) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.LoginAsync(
            command.Email,
            command.Password);

        if (result.Failed)
            return result.To<LoginResponse>();

        return new LoginResponse(
            result.Value.AccessToken,
            result.Value.RefreshToken,
            result.Value.AccessTokenExpirationInMinutes,
            result.Value.RefreshTokenExpirationInHours);
    }
}

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int AccessTokenExpirationInMinutes,
    int RefreshTokenExpirationInHours);

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(UserErrors.EmailRequired.Message)
            .EmailAddress()
            .WithMessage(x => UserErrors.InvalidEmailFormat(x.Email).Message)
            .MaximumLength(DomainConstants.User.MaxEmailLength)
            .WithMessage(UserErrors.EmailTooLong.Message);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(UserErrors.PasswordRequired.Message)
            .MaximumLength(DomainConstants.User.MaxPasswordLength)
            .WithMessage(UserErrors.PasswordTooLong.Message);
    }
}
