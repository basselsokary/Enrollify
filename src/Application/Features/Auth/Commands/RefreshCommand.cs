using Application.Common.Interfaces.Authentication;
using Domain.Common;
using Domain.Common.Constants;
using FluentValidation;

namespace Application.Features.Auth.Commands;

public sealed record RefreshCommand(
    string RefreshToken) : ICommand<RefreshResponse>;

public sealed class RefreshCommandHandler(
    IIdentityService identityService) : ICommandHandler<RefreshCommand, RefreshResponse>
{
    public async Task<Result<RefreshResponse>> HandleAsync(RefreshCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.RefreshTokenAsync(
            command.RefreshToken);
        
        if (result.Failed)
            return result.To<RefreshResponse>();

        return new RefreshResponse(
            result.Value.AccessToken,
            result.Value.RefreshToken,
            result.Value.AccessTokenExpirationInMinutes,
            result.Value.RefreshTokenExpirationInHours);
    }
}

public sealed record RefreshResponse(
    string AccessToken,
    string RefreshToken,
    int AccessTokenExpirationInMinutes,
    int RefreshTokenExpirationInHours);

internal sealed class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage(ValidationErrors.ValueRequired.Message)
            .MaximumLength(DomainConstants.User.MaxRefreshTokenLength)
            .WithMessage(ValidationErrors.MaximumLengthExceeded.Message);
    }
}
