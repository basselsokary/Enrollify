using Application.Contracts.Users;

namespace Application.Common.Interfaces.Authentication;

public interface IUserContext
{
    Guid Id { get; }
    string UserName { get; }
    bool IsInRoles(params string[] roles);
    bool IsInAnyRole(params string[] roles);
    bool IsAuthenticated { get; }
    UserDto User { get; }
}
