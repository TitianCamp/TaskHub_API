using System.Security.Claims;

namespace TaskHub.Business.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(string userId, string email, string role);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
