namespace TaskHub.Business.DataModels;

public record JwtSettings(
    string Key,
    string Issuer,
    string Audience,
    int AccessTokenExpirationMinutes,
    int RefreshTokenExpirationDays)
{ }
