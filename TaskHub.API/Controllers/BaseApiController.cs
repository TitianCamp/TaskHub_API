using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskHub.Business.Results;

namespace TaskHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseApiController : ControllerBase
    {
        protected Guid? GetCurrentUser()
        {
            string? userIdString = User.Claims.FirstOrDefault(
                c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }

            return null;
        }

        protected IActionResult SendResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value!);
            }

            return BadRequest(result.Error!);
        }
    }
}
