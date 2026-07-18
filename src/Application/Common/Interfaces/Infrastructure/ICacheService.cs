namespace Application.Common.Interfaces.Infrastructure;

public interface ICacheService
{
    Task RemoveAsync(string key);
    Task RemoveByPrefixAsync(string prefix);
    Task RemoveByIdAsync(string prefix, string id);
}