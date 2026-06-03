using TaskHub.Data.Entities;

namespace TaskHub.Data.Repositories.Interfaces;

public interface IRefreshTokenRepository
    : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenValue(string token);
    Task ReplaceRefreshToken(Guid oldTokenId, RefreshToken newToken);
}
