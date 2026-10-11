using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Data.Repositories;

public class TeamRepository(AppDbContext context) 
    : ITeamRepository
{
    public async Task<IReadOnlyList<Team>> GetAll()
    {
        return await context.Teams
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Team?> GetById(Guid id)
    {
        return await context.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task Add(Team entity)
    {
        await context.Teams.AddAsync(entity);

        await context.SaveChangesAsync();
    }

    public async Task Update(Team entity)
    {
        Team? trackedEntity = context.Teams.Local
            .FirstOrDefault(t => t.Id == entity.Id);

        if (trackedEntity != null)
        {
            context.Entry(trackedEntity).State = EntityState.Detached;
        }

        context.Teams.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        Team? team = await context.Teams.FindAsync(id);

        if (team != null)
        {
            context.Teams.Remove(team);
            await context.SaveChangesAsync();
        }
    }

    public async Task<Team?> GetWithMembers(Guid teamId)
    {
        return await context.Teams
            .Include(t => t.TeamMembers)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == teamId);
    }

    public async Task RemoveMember(TeamMember membership)
    {
        context.TeamMembers.Remove(membership);
        await context.SaveChangesAsync();
    }

    public async Task AddMember(TeamMember member)
    {
        await context.TeamMembers.AddAsync(member);
        await context.SaveChangesAsync();
    }

    public async Task<bool> IsMember(Guid userId, Guid teamId)
    {
        TeamMember? found = await context
            .TeamMembers.FindAsync(userId, teamId);

        return found != null;
    }

    public async Task<bool> IsOwner(Guid userId, Guid teamId)
    {
        return await context
            .TeamMembers.AnyAsync(tm => 
                tm.UserId ==  userId && 
                tm.TeamId == teamId && 
                tm.Role == TeamRole.Owner);
    }
}
