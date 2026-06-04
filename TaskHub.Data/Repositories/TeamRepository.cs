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

    public async Task<Team?> GetById(Guid id)
    {
        return await _context.Teams
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

    public async Task<Team?> GetWithMembers(Guid teamId)
    {
        return await _context.Teams
            .Include(t => t.TeamMembers)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == teamId);
    }

    public async Task RemoveMember(TeamMember membership)
    {
        _context.TeamMembers.Remove(membership);
        await _context.SaveChangesAsync();
    }

    public async Task AddMember(TeamMember member)
    {
        await _context.TeamMembers.AddAsync(member);
        await _context.SaveChangesAsync();
    }
}
