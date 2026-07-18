using Application.Common.Interfaces.Authentication;
using Domain.Common;
using Domain.Common.Constants;
using FluentValidation;

namespace Application.Features.Auth.Commands;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password) : ICommand;

public sealed class RegisterCommandHandler(
    IIdentityService identityService) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> HandleAsync(RegisterCommand command, CancellationToken cancellationToken)
    {
        Guid userId = Guid.NewGuid();
        
        if (await identityService.IsUserExist(command.Email, cancellationToken))
            // Return success to avoid exposing whether the email is already taken or not,
            // which can be a security risk.
            return Result.Success();

        var result = await identityService.RegisterAsync(
            userId,
            command.FullName,
            command.Email,
            command.Password,
            UserRoles.User);
        
        return result;
    }
}

internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(UserErrors.FullNameRequired.Message)
            .MaximumLength(DomainConstants.User.MaxFullNameLength)
            .WithMessage(UserErrors.FullNameExceededLength.Message);

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
            .MinimumLength(DomainConstants.User.MinPasswordLength)
            .WithMessage(UserErrors.PasswordTooShort.Message)
            .MaximumLength(DomainConstants.User.MaxPasswordLength)
            .WithMessage(UserErrors.PasswordTooLong.Message);
    }
}