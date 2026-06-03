using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Data.Repositories;

public class TeamRepository
    : ITeamRepository
{
    private readonly AppDbContext _context;

    public TeamRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Team>> GetAll()
    {
        return await _context.Teams
            .Include(t => t.Owner)
            .AsNoTracking()
            .ToListAsync();
    }

    public Task<Team?> GetById(Guid id)
    {
        return _context.Teams
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task Add(Team entity)
    {
        await _context.Teams.AddAsync(entity);

        await _context.SaveChangesAsync();
    }

    public async Task Update(Team entity)
    {
        Team? trackedEntity = _context.Teams.Local
            .FirstOrDefault(t => t.Id == entity.Id);

        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).State = EntityState.Detached;
        }

        _context.Teams.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        Team? team = await _context.Teams.FindAsync(id);

        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddUserToTeam(Guid teamId, Guid userId)
    {
        Team? team = await _context.Teams
            .Include(t => t.TeamMembers)
            .FirstOrDefaultAsync(t => t.Id == teamId);

        if (team != null)
        {
            team.TeamMembers.Add(
                new TeamMember(userId, teamId));

            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveMember(Guid teamId, Guid userId)
    {
        TeamMember? membership = await _context.TeamMembers
            .FirstOrDefaultAsync(tm => tm.UserId == userId && tm.TeamId == teamId);

        if (membership != null)
        {
            _context.TeamMembers.Remove(membership);
            await _context.SaveChangesAsync();
        }
    }
}
