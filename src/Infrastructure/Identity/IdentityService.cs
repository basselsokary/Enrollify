using Application.Common.Interfaces.Authentication;
using Application.Contracts.Auth;
using Domain.Common.Shared;
using Infrastructure.Authentication;
using Infrastructure.Persistence.WriteContext.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity;

internal class IdentityService(
    UserManager<Participant> userManager,
    SignInManager<Participant> signInManager,
    JwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtOptions> jwtSettings,
    WriteDbContext appContext) : IIdentityService
{
    private readonly JwtOptions _jwtSettings = jwtSettings.Value;

    public async Task<Result> RegisterAsync(Guid userId, string fullname, string email, string password, string role)
    {
        string normalizedEmail = userManager.NormalizeEmail(email);
        if (await userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail))
            return ParticipantErrors.EmailAlreadyInUse;

        var participantResult = Participant.Create(userId, fullname, email);
        if (participantResult.Failed)
            return participantResult;
        
        var participant = participantResult.Value;

        var result = await userManager.CreateAsync(participant, password);

        if (!result.Succeeded)
            return ValidationError.FromErrors(result.Errors.Select(e => Error.Validation(e.Code, e.Description)));

        var identityResult = await userManager.AddToRoleAsync(participant, role);
        if (!identityResult.Succeeded)
        {
            await userManager.DeleteAsync(participant);
            return ValidationError.FromErrors(identityResult.Errors.Select(e => Error.Validation(e.Code, e.Description)));
        }

        return Result.Success(participant);
    }

    public async Task<Result<AuthenticatedDto>> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return ParticipantErrors.InvalidCredentials;

        if (!user.IsActive)
            return ParticipantErrors.InActiveUser;

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            return signInResult.IsLockedOut
                ? ParticipantErrors.LockedAccount
                : ParticipantErrors.InvalidCredentials;
        }

        user.RecordLastLogin();
        await userManager.UpdateAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = jwtTokenGenerator.GenerateRefreshToken();

        var saveTokenResult = await AddRefreshTokenAsync(user.Id, refreshToken);
        if (saveTokenResult.Failed)
            return saveTokenResult.Error;

        var response = new AuthenticatedDto(
            user.Id,
            accessToken,
            refreshToken,
            _jwtSettings.AccessTokenExpirationInMinutes,
            _jwtSettings.RefreshTokenExpirationInHours);
        
        return Result.Success(response);
    }

    public async Task<Result<AuthenticatedDto>> RefreshTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await appContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedRefreshToken == null || !storedRefreshToken.IsActive)
        {
            return ParticipantErrors.InvalidRefreshToken;
        }

        var user = await userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
        if (user == null || !user.IsActive)
        {
            return ParticipantErrors.InActiveUser;
        }

        // Revoke old refresh token
        storedRefreshToken.Revoke();

        var roles = await userManager.GetRolesAsync(user);
        
        var newAccessToken = jwtTokenGenerator.GenerateAccessToken(user, roles);
        var newRefreshToken = jwtTokenGenerator.GenerateRefreshToken();

        await AddRefreshTokenAsync(user.Id, newRefreshToken);
        await appContext.SaveChangesAsync();

        var response = new AuthenticatedDto(
            user.Id,
            newAccessToken,
            newRefreshToken,
            _jwtSettings.AccessTokenExpirationInMinutes,
            _jwtSettings.RefreshTokenExpirationInHours);
        
        return Result.Success(response);
    }

    public async Task<bool> IsUserExist(string email, CancellationToken cancellationToken)
    {
        string normalizedEmail = userManager.NormalizeEmail(email);
        var existingUser = await userManager.Users.AnyAsync(
            u => u.NormalizedEmail == normalizedEmail, cancellationToken);
        
        return existingUser;
    }

    private async Task<Result> AddRefreshTokenAsync(Guid userId, string token)
    {
        var refreshTokenResult = RefreshToken.Create(
            token,
            userId,
            DateTime.UtcNow.AddHours(_jwtSettings.RefreshTokenExpirationInHours));

        if (refreshTokenResult.Failed)
            return refreshTokenResult;

        appContext.RefreshTokens.Add(refreshTokenResult.Value);

        return Result.Success();
    }
}
