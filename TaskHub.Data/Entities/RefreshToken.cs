namespace TaskHub.Data.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;

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
