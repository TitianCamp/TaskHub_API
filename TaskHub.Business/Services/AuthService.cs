using Microsoft.AspNetCore.Identity;
using TaskHub.Business.DataModels;
using TaskHub.Business.DataModels.User;
using TaskHub.Business.Services.Interfaces;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using TaskHub.Business.Mappers;
using TaskHub.Business.Results;
using TaskHub.Business.Validators;

namespace TaskHub.Business.Services;

public class AuthService : IAuthService
{
    private const string DEFAULT_ROLE = "User";

    private readonly JwtSettings _jwtSettings;
    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    private readonly IUserValidator _validator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(JwtSettings jwtSettings,
        IUserValidator validator,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService, UserManager<User> userManager)
    {
        _jwtService = jwtService;
        _userManager = userManager;
        _validator = validator;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtSettings;
    }

    public async Task<Result<TokensPair>> Login(
        UserLoginRequest request)
    {
        Result<bool> validateResult = _validator.Validate(request);

        if (!validateResult.IsSuccess)
        {
            return Result.Failure<TokensPair>(validateResult.Error!);
        }

        User? user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure<TokensPair>("User with this email does not exist");
        }

        bool passwordCheckResult = await
            _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordCheckResult)
        {
            return Result.Failure<TokensPair>("Incorrect password");
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string role = roles.FirstOrDefault() ?? DEFAULT_ROLE;

        string accessToken = _jwtService.GenerateAccessToken(user.Id.ToString(), user.Email!, role);
        string refreshToken = _jwtService.GenerateRefreshToken();

        RefreshToken refreshTokenEntity = new(
            refreshToken,
            DateTime.UtcNow.AddDays(
                _jwtSettings.RefreshTokenExpirationDays),
            user.Id);


        await _refreshTokenRepository.Add(refreshTokenEntity);

        return Result.Success(
            new TokensPair(
            accessToken, refreshToken,
            refreshTokenEntity.ExpiresAt));
    }

    public async Task<Result<UserRegisterResponce>> Register(
        UserRegisterRequest request)
    {
        Result<bool> validateResult = _validator.Validate(request);
        if (!validateResult.IsSuccess)
        {
            return Result.Failure<UserRegisterResponce>(validateResult.Error!);
        }

        if (_userManager.Users.Any(u => u.Email == request.Email))
        {
            return Result.Failure<UserRegisterResponce>(
                "User with this email already exists");
        }

        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            string suffix = Guid.NewGuid().ToString("N").Substring(0, 5);
            request.UserName = $"{request.FirstName}_{suffix}";
        }
        else
        {
            User? existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
            {
                return Result.Failure<UserRegisterResponce>(
                    "User with this username already exists");
            }
        }

        User user = request.ToEntity();

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return Result.Failure<UserRegisterResponce>(createResult.Errors.First().Description);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, DEFAULT_ROLE);
        if (!roleResult.Succeeded)
        {
            return Result.Failure<UserRegisterResponce>("User was created, but failed adding role");
        }

        return Result.Success(user.ToDto(DEFAULT_ROLE));
    }

    public async Task<Result<TokensPair>> Refresh(
        string accessToken, string refreshToken)
    {
        ClaimsPrincipal? principal = _jwtService.GetPrincipalFromExpiredToken(accessToken);

        if (principal == null)
        {
            return Result.Failure<TokensPair>("Invalid access token");
        }

        UserDataFromJwt? userData = GetUserDataFromClaims(principal.Claims);
        if (userData == null)
        {
            return Result.Failure<TokensPair>("Invalid access token claims");
        }

        RefreshToken? oldRefreshToken = await _refreshTokenRepository.GetByTokenValue(refreshToken);
        if (oldRefreshToken == null || oldRefreshToken.UserId.ToString() != userData.UserId)
        {
            return Result.Failure<TokensPair>("Invalid refresh token");
        }
        if (oldRefreshToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result.Failure<TokensPair>("Refresh token has expired");
        }

        string newAccessToken = _jwtService.GenerateAccessToken(
            userData.UserId, userData.Email, userData.Role);
        string newRefreshToken = _jwtService.GenerateRefreshToken();

        RefreshToken newRefreshTokenEntity = new(
            newRefreshToken,
            DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            Guid.Parse(userData.UserId));

        try
        {
            await _refreshTokenRepository.ReplaceRefreshToken(
                oldRefreshToken.Id, newRefreshTokenEntity);
        }
        catch
        {
            return Result.Failure<TokensPair>("An error occurred while updating the session.");
        }

        return Result.Success(
            new TokensPair(
                newAccessToken, 
                newRefreshToken, 
                newRefreshTokenEntity.ExpiresAt));
    }

    public async Task<Result<string>> Logout(
        string token)
    {
        RefreshToken? refreshToken = await
            _refreshTokenRepository.GetByTokenValue(token);

        if (refreshToken == null)
        {
            return Result.Failure<string>("Invalid refresh token");
        }

        try
        {
            await _refreshTokenRepository.Delete(refreshToken.Id);

            return Result.Success("Logout successful");
        }
        catch
        {
            return Result.Failure<string>("An error occurred while logging out");
        }
    }

    private UserDataFromJwt? GetUserDataFromClaims(
        IEnumerable<Claim> claims)
    {
        string? userId = claims.FirstOrDefault(
            c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        string? email = claims.FirstOrDefault(
            c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
        string? role = claims.FirstOrDefault(
            c => c.Type == ClaimTypes.Role)?.Value;

        if (userId == null || email == null || role == null)
        {
            return null;
        }

        return new UserDataFromJwt(userId, email, role);
    }
}
