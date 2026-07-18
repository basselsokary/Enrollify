namespace Application.Common.Interfaces.Authentication;

public interface IUserContext
{
    Guid Id { get; }
    string UserName { get; }
    bool IsAuthenticated { get; }
}
