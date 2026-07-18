using Domain.Common.Shared;

namespace Infrastructure.Identity;

public static class ParticipantErrors
{
    public static Error NotFound(string key)
        => Error.NotFound("User.NotFound", $"User with '{key}' was not found.");

    public static Error FullNameRequired =>
        Error.Validation("User.FullNameRequired", "Full name is required.");
    public static Error EmailRequired =>
        Error.Validation("User.EmailRequired", "Email is required.");
    public static Error InvalidCredentials =>
        Error.Failure("User.InvalidCredentials", "Email or password is wrong.");
    public static Error EmailAlreadyInUse =>
        Error.Conflict("User.EmailAlreadyInUse", "User with this email already exists.");

    public static Error InActiveUser =>
        Error.Failure("User.InActiveUser", "Account is deactivated.");
    public static Error LockedAccount =>
        Error.Failure("User.LockedAccount", "Account is locked out.");

    public static Error RefreshTokenRequired
        => Error.Validation("User.RefreshTokenRequired", "A valid refresh token is required to refresh the access token.");
    public static Error RefreshTokenExpirationInvalid
        => Error.Validation("User.RefreshTokenExpirationInvalid", "The provided refresh token has expired.");
    public static Error InvalidRefreshToken =>
        Error.Failure("User.InvalidRefreshToken", "Invalid or expired refresh token.");
}