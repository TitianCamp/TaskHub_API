using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Data.Repositories;

public class UserRepository(
    AppDbContext context) : IUserRepository
{
    public async Task<IReadOnlyList<User>> GetAll()
    {
        return await context.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetById(Guid id)
    {
        return await context.Users
            .AsNoTracking()
            .Include(t => t.AuthoredTasks)
            .AsSplitQuery()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task Add(User entity)
    {
        await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Update(User entity)
    {
        User? trackedEntity = context.Users.Local
            .FirstOrDefault(u => u.Id == entity.Id);

        if (trackedEntity != null)
        {
            context.Entry(trackedEntity).State = EntityState.Detached;
        }

        context.Users.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        User? user = await context.Users.FindAsync(id);

        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
