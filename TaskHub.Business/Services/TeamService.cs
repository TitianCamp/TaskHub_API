using TaskHub.Business.DataModels.Team;
using TaskHub.Business.Mappers;
using TaskHub.Business.Results;
using TaskHub.Business.Validators;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Business.Services;

public class TeamService
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

    public async Task<Result<TeamResponce>> CreateTeam(TeamRequest request)
    {
        Result<bool> validationResult = _validator.Validate(request);
        if (!validationResult.IsSuccess)
        {
            return Result.Failure<TeamResponce>(validationResult.Error!);
        }

        try
        {
            Team team = request.ToEntity();
            await _teamRepository.Add(team);
            return Result.Success(team.ToDto());
        }
        catch
        {
            return Result.Failure<TeamResponce>(
                "An error occurred while creating the team");
        }
    }
    public async Task<Result<string>> DeleteTeam(Guid id, string userId)
    {
        try
        {
            Team? team = await _teamRepository.GetById(id);
            if (team == null)
            {
                return Result.Failure<string>("Team not found");
            }

            if (!userId.Equals(team.OwnerId.ToString()))
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
}
