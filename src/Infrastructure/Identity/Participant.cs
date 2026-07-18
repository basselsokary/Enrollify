using Domain.Common.Shared;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

internal class Participant : IdentityUser<Guid>
{
    private Participant() { }

    public string FullName { get; private set; } = null!;
    public UserStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsDeleted => DeletedAt.HasValue;
    public bool IsLocked => LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.UtcNow;

    public static Result<Participant> Create(
        Guid userId,
        string fullName,
        string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return ParticipantErrors.FullNameRequired;

        if (string.IsNullOrWhiteSpace(email))
            return ParticipantErrors.EmailRequired;

        var participant = new Participant
        {
            Id = userId,
            FullName = fullName,
            Email = email,
            UserName = email,
            CreatedAt = DateTime.UtcNow,
            Status = UserStatus.Active,
            IsActive = true
        };

        return Result.Success(participant);
    }

    public void RecordLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void SuspendAccount()
    {
        Status = UserStatus.Suspended;
        IsActive = false;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        IsActive = true;
    }

    public void MarkDeleted()
    {
        DeletedAt = DateTime.UtcNow;
        Status = UserStatus.Deleted;
        IsActive = false;
    }
}
