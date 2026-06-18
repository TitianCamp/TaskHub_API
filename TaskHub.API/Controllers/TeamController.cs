using Microsoft.AspNetCore.Mvc;

namespace TaskHub.API.Controllers;

public class TeamController : BaseApiController
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _teamService.GetAllTeams();

        return SendResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _teamService.GetTeamById(id);

        return SendResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam(
        [FromBody] TeamRequest request)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _teamService.CreateTeam(request, userId.Value);
        return SendResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _teamService.DeleteTeam(id, userId.Value);
        return SendResult(result);
    }

    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinTeam(Guid id)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _teamService.JoinTeam(id, userId.Value);
        return SendResult(result);
    }

    [HttpPost("{id}/leave")]
    public async Task<IActionResult> LeaveTeam(Guid id)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _teamService.LeaveTeam(id, userId.Value);
        return SendResult(result);
    }
}
