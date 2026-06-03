using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Data.Repositories;

public class RefreshTokenRepository
    : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RefreshToken>> GetAll()
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<RefreshToken?> GetById(Guid id)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<RefreshToken?> GetByTokenValue(string token)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task Add(RefreshToken entity)
    {
        await _context.RefreshTokens.AddAsync(entity);

        await _context.SaveChangesAsync();
    }

    public async Task Update(RefreshToken entity)
    {
        RefreshToken? trackedEntity = _context.RefreshTokens.Local
            .FirstOrDefault(rt => rt.Id == entity.Id);

        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).State = EntityState.Detached;
        }

        _context.RefreshTokens.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        RefreshToken? token = await _context.RefreshTokens.FindAsync(id);

        if (token != null)
        {
            _context.RefreshTokens.Remove(token);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ReplaceRefreshToken(Guid oldTokenId, RefreshToken newToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            RefreshToken? oldToken = await _context.RefreshTokens.FindAsync(oldTokenId);
            if (oldToken != null)
            {
                _context.RefreshTokens.Remove(oldToken);
            }
            await _context.RefreshTokens.AddAsync(newToken);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw new Exception("Replacing refresh token failed!");
        }
    }
}
