using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using TaskHub.Business.Results;

namespace TaskHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TeamController : ControllerBase
{
    private readonly TeamService _teamService;

    public TeamController(TeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Result<List<TeamResponce>> result = await
            _teamService.GetAllTeams();

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return BadRequest(result.Error);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Result<TeamResponce> result = await _teamService.GetTeamById(id);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return BadRequest(result.Error);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTeam(
        [FromBody] TeamRequest request)
    {
        string? userId = User.Claims.FirstOrDefault(
            c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        request.OwnerId = Guid.Parse(userId);

        Result<TeamResponce> result = await
            _teamService.CreateTeam(request);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return BadRequest(result.Error);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(Guid id)
    {
        string? userId = User.Claims.FirstOrDefault(
            c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        Result<string> result = await _teamService.DeleteTeam(id, userId);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return BadRequest(result.Error);
        }
    }

    [HttpPost("{id}/join")]
    public IActionResult JoinTeam(Guid id)
    {
        return Ok($"Join team with ID: {id}");
    }

    [HttpPost("{id}/leave")]
    public IActionResult LeaveTeam(Guid id)
    {
        return Ok($"Leave team with ID: {id}");
    }
}
