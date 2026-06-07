using TaskHub.Business.DataModels.Team;
using TaskHub.Business.Results;

namespace TaskHub.Business.Services.Interfaces;

public interface ITeamService
{
    Task<Result<List<TeamResponce>>> GetAllTeams();
    Task<Result<TeamResponce>> GetTeamById(Guid id);
    Task<Result<TeamResponce>> CreateTeam(TeamRequest request, Guid userId);
    Task<Result<string>> DeleteTeam(Guid id, Guid userId);
    Task<Result<string>> JoinTeam(Guid teamId, Guid userId);
    Task<Result<string>> LeaveTeam(Guid teamId, Guid userId);
}
