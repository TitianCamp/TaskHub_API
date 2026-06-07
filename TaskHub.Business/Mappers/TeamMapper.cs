using TaskHub.Business.DataModels.Team;
using TaskHub.Data.Entities;

namespace TaskHub.Business.Mappers;

public static class TeamMapper
{
    public static TeamResponce ToDto(this Team team)
    {
        return new TeamResponce
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
        };
    }

    public static Team ToEntity(this TeamRequest request)
    {
        return new Team(
            request.Name,
            request.Description);
    }
}
