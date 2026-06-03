namespace TaskHub.Business.DataModels;

public record TokensPair(
    string AccessToken,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt)
{ }
