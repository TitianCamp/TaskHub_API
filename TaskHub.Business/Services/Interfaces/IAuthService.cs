using TaskHub.Business.DataModels;
using TaskHub.Business.DataModels.User;
using TaskHub.Business.Results;

namespace TaskHub.Business.Services.Interfaces;

public interface IAuthService
{
    Task<Result<TokensPair>> Login(UserLoginRequest request);
    Task<Result<UserRegisterResponce>> Register(UserRegisterRequest request);
    Task<Result<TokensPair>> Refresh(string accessToken, string refreshToken);
    Task<Result<string>> Logout(string token);
}
