using Domain.Common.Shared;
using Infrastructure.Identity;

namespace Infrastructure.Authentication;

internal class RefreshToken
{
    public const int MaxRefreshTokenLength = 128;

    private RefreshToken() { }
    private RefreshToken(string token, Guid userId, DateTime expiresAt)
    {
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;

        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    public Guid Id { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsActive => !IsRevoked && (DateTime.UtcNow < ExpiresAt);

    public static Result<RefreshToken> Create(string token, Guid userId, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            return ParticipantErrors.RefreshTokenRequired;

        if (expiresAt <= DateTime.UtcNow)
            return ParticipantErrors.RefreshTokenExpirationInvalid;
        
        return new RefreshToken(token, userId, expiresAt);
    }

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}
