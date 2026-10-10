namespace TaskHub.Data.Entities;

public class RefreshToken
{
    public Guid Id { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public Guid UserId { get; init; }
    public DateTime CreatedAt { get; init; }

    public User User { get; init; } = null!;

    public RefreshToken()
    { }

    public RefreshToken(string token, DateTime expiresAt, Guid userId)
    {
        Id = Guid.NewGuid();
        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }
}
