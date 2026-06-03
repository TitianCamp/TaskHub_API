using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Data.Repositories;

public class UserRepository
    : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetById(Guid id)
    {
        return await _context.Users
            .Include(t => t.AuthoredTasks)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task Add(User entity)
    {
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(User entity)
    {
        User? trackedEntity = _context.Users.Local
            .FirstOrDefault(u => u.Id == entity.Id);

        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).State = EntityState.Detached;
        }

        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        User? user = await _context.Users.FindAsync(id);

        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
