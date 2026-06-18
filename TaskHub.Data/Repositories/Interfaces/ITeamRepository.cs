using TaskHub.Data.Entities;

namespace TaskHub.Data.Repositories.Interfaces;

public interface ITeamRepository
    : IRepository<Team>
{
    Task<Team?> GetWithMembers(Guid teamId);

    Task RemoveMember(TeamMember membership);
    Task AddMember(TeamMember member);
    Task<bool> IsMember(Guid userId, Guid teamId);
}
