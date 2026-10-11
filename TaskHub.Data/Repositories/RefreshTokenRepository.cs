using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Data.Repositories;

public class RefreshTokenRepository(
    AppDbContext context)
    : IRefreshTokenRepository
{

    public async Task<IReadOnlyList<RefreshToken>> GetAll()
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<RefreshToken?> GetById(Guid id)
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByTokenValue(string token)
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task Add(RefreshToken entity)
    {
        await context.RefreshTokens.AddAsync(entity);

        await context.SaveChangesAsync();
    }

    public async Task Update(RefreshToken entity)
    {
        RefreshToken? trackedEntity = context.RefreshTokens.Local
            .FirstOrDefault(rt => rt.Id == entity.Id);

        if (trackedEntity != null)
        {
            context.Entry(trackedEntity).State = EntityState.Detached;
        }

        context.RefreshTokens.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        RefreshToken? token = await context.RefreshTokens.FindAsync(id);

        if (token != null)
        {
            context.RefreshTokens.Remove(token);
            await context.SaveChangesAsync();
        }
    }

    public async Task ReplaceRefreshToken(Guid oldTokenId, RefreshToken newToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            RefreshToken? oldToken = await context.RefreshTokens.FindAsync(oldTokenId);
            if (oldToken != null)
            {
                context.RefreshTokens.Remove(oldToken);
            }
            await context.RefreshTokens.AddAsync(newToken);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new Exception("Replacing refresh token failed!");
        }
    }
}
