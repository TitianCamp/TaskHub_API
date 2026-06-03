using Microsoft.AspNetCore.Mvc;
using TaskHub.Business.Results;

namespace TaskHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] UserRegisterRequest request)
    {
        Result<UserRegisterResponce> result = await _authService.Register(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginRequest request)
    {
        Result<TokensPair> tokens = await _authService.Login(request);
        if (!tokens.IsSuccess)
        {
            return BadRequest(tokens.Error);
        }

        Response.Cookies.Append("refreshToken", tokens.Value!.RefreshToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            Expires = tokens.Value.RefreshTokenExpiresAt
        });

        return Ok(new {AccessToken = tokens.Value!.AccessToken});
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] UserRefreshRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AccessToken))
        {
            return BadRequest("No access token provided");
        }

        string? refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return BadRequest("No refresh token provided");
        }

        Result<TokensPair> tokens = await _authService.Refresh(request.AccessToken, refreshToken);

        if (!tokens.IsSuccess)
        {
            return BadRequest(tokens.Error);
        }

        Response.Cookies.Append("refreshToken", tokens.Value!.RefreshToken, new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            Expires = tokens.Value.RefreshTokenExpiresAt
        });

        return Ok(new { AccessToken = tokens.Value!.AccessToken });

    }

    [HttpDelete("logout")]
    public async Task<IActionResult> Logout()
    {
        string? token = Request.Cookies["refreshToken"];

        if (string.IsNullOrWhiteSpace(token))
        {
            return BadRequest("No refresh token provided");
        }

        Result<string> result = await _authService.Logout(token);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(result.Value);
    }
}
