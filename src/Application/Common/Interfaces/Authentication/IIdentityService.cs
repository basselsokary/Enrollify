using Application.Contracts.Auth;
using Domain.Common.Shared;

namespace Application.Common.Interfaces.Authentication;

public interface IIdentityService
{
    Task<Result> RegisterAsync(Guid userId, string fullname, string email, string password, string role);
    Task<Result<AuthenticatedDto>> LoginAsync(string email, string password);
    Task<Result<AuthenticatedDto>> RefreshTokenAsync(string refreshToken);
    Task<bool> IsUserExist(string email, CancellationToken cancellationToken);
}
