using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskHub.Business.Results;

namespace TaskHub.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpPost]
    public async Task<IActionResult> CreateTeam(
        [FromBody] TeamRequest request)
    {
        string? userId = User.Claims.FirstOrDefault(
            c => c.Type == ClaimTypes.NameIdentifier)?.Value;
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
            c => c.Type == ClaimTypes.NameIdentifier)?.Value;
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
    public async Task<IActionResult> JoinTeam(Guid id)
    {
        string? userId = User.Claims.FirstOrDefault(
            c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        Result<string> result = await _teamService.JoinTeam(id, Guid.Parse(userId));
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return BadRequest(result.Error);
        }
    }

    [HttpPost("{id}/leave")]
    public async Task<IActionResult> LeaveTeam(Guid id)
    {
        string? userId = User.Claims.FirstOrDefault(
            c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        Result<string> result = await _teamService.LeaveTeam(id, Guid.Parse(userId));
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
        else
        {
            return BadRequest(result.Error);
        }
    }
}
