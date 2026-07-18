using Application.Common.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Authentication;

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid Id
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var id = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            
            bool successfulParse = Guid.TryParse(id, out Guid guidId);
            if (successfulParse)
                return guidId;

            throw new UnauthorizedAccessException("Invalid user ID.");
        }
    }
    
    public string UserName
    {
        get
        {
            var fullName = httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.PreferredUsername);
            
            return fullName ?? "System";
        }
    }

    public bool IsAuthenticated
        => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    internal Guid UserId
    {
        get
        {
            string? id = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool successfulParse = Guid.TryParse(id, out Guid guidId);
            return successfulParse ? guidId : default;
        }
    }
}
