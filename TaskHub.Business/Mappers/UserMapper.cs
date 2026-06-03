using TaskHub.Business.DataModels.User;
using TaskHub.Data.Entities;

namespace TaskHub.Business.Mappers;

public static class UserMapper
{
    public static User ToEntity(this UserRegisterRequest request)
    {
        return new User(
            request.UserName!,
            request.FirstName,
            request.LastName,
            request.Email);
    }

    public static UserRegisterResponce ToDto(this User user, string role)
    {
        return new UserRegisterResponce
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            Role = role
        };
    }
}
