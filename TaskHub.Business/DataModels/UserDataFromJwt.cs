namespace TaskHub.Business.DataModels;

public record UserDataFromJwt(
    string UserId,
    string Email,
    string Role)
{ }
