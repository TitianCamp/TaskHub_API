using TaskHub.Business.DataModels.Team;
using TaskHub.Business.Mappers;
using TaskHub.Business.Results;
using TaskHub.Business.Services.Interfaces;
using TaskHub.Business.Validators;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Business.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IValidator<TeamRequest> _validator;

    public TeamService(ITeamRepository teamRepository,
        IValidator<TeamRequest> validator)
    {
        _teamRepository = teamRepository;
        _validator = validator;
    }

    public async Task<Result<List<TeamResponce>>> GetAllTeams()
    {
        try
        {
            IEnumerable<Team> teams = await _teamRepository.GetAll();

            if (teams == null || !teams.Any())
            {
                return Result.Failure<List<TeamResponce>>("No teams found");
            }

            List<TeamResponce> teamResponces = teams
                .Select(t => t.ToDto()).ToList();

            return Result.Success(teamResponces);
        }
        catch
        {
            return Result.Failure<List<TeamResponce>>(
                "An error occurred while retrieving teams");
        }
    }

    public async Task<Result<TeamResponce>> GetTeamById(Guid id)
    {
        try
        {
            Team? team = await _teamRepository.GetById(id);

            if (team == null)
            {
                return Result.Failure<TeamResponce>("Team not found");
            }

            return Result.Success(team.ToDto());
        }
        catch
        {
            return Result.Failure<TeamResponce>(
                "An error occurred while retrieving the team");
        }
    }

    public async Task<Result<TeamResponce>> CreateTeam(TeamRequest request, Guid userId)
    {
        Result<bool> validationResult = _validator.Validate(request);
        if (!validationResult.IsSuccess)
        {
            return Result.Failure<TeamResponce>(validationResult.Error!);
        }

        try
        {
            Team team = request.ToEntity();
            TeamMember owner = new(userId, team.Id, TeamRole.Owner);
            team.TeamMembers.Add(owner);

            await _teamRepository.Add(team);
            return Result.Success(team.ToDto());
        }
        catch
        {
            return Result.Failure<TeamResponce>(
                "An error occurred while creating the team");
        }
    }

    public async Task<Result<string>> DeleteTeam(Guid id, Guid userId)
    {
        try
        {
            Team? team = await _teamRepository.GetById(id);
            if (team == null)
            {
                return Result.Failure<string>("Team not found");
            }

            if (!team.TeamMembers.Any(tm => tm.UserId == userId && tm.Role == TeamRole.Owner))
            {
                return Result.Failure<string>("Only owner can delete team");
            }

            await _teamRepository.Delete(id);
            return Result.Success("Team deleted successfully");
        }
        catch
        {
            return Result.Failure<string>("An error occurred while deleting the team");
        }
    }

    public async Task<Result<string>> JoinTeam(Guid teamId, Guid userId)
    {
        Team? team = await _teamRepository.GetWithMembers(teamId);
        if (team == null)
        {
            return Result.Failure<string>("This team doesn`t exist");
        }

        if (team.TeamMembers.Any(tm => tm.UserId == userId))
        {
            return Result.Failure<string>("You are already in a team");
        }

        try
        {
            await _teamRepository.AddMember(new TeamMember(userId, teamId));
            return Result.Success("User was added to team");
        }
        catch
        {
            return Result.Failure<string>("Error occured while adding to team");
        }
    }

    public async Task<Result<string>> LeaveTeam(Guid teamId, Guid userId)
    {
        Team? team = await _teamRepository.GetWithMembers(teamId);
        if (team == null)
        {
            return Result.Failure<string>("This team doesn`t exist");
        }

        if (team.TeamMembers.Any(tm=>tm.UserId == userId && tm.Role == TeamRole.Owner))
        {
            return Result.Failure<string>("Owner can't leave team");
        }

        if (!team.TeamMembers.Any(tm => tm.UserId == userId))
        {
            return Result.Failure<string>("You are not a member of this team");
        }

        try
        {
            await _teamRepository.RemoveMember(new TeamMember(userId, teamId));
            return Result.Success("User was removed from team");
        }
        catch
        {
            return Result.Failure<string>("Error occured while removing from team");
        }
    }
}
