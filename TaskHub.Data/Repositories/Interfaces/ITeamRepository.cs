using TaskHub.Data.Entities;

namespace TaskHub.Data.Repositories.Interfaces;

public interface ITeamRepository
    : IRepository<Team>
{
    Task AddUserToTeam(Guid teamId, Guid userId);
    Task RemoveMember(Guid teamId, Guid userId);
}
