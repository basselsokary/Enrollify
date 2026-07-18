namespace Application.Contracts.Auth;

public sealed record AuthenticatedDto(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    int AccessTokenExpirationInMinutes,
    int RefreshTokenExpirationInHours);