namespace TaskHub.Data.Entities;

public class TeamMember
{
    public Guid UserId { get; init; }
    public Guid TeamId { get; init; }
    public TeamRole Role { get; init; }
    public DateTime CreatedAt { get; init; }

    public User User { get; init; } = null!;
    public Team Team { get; init; } = null!;

    public TeamMember()
    { }

    public TeamMember(Guid userId, Guid teamId,
        TeamRole role = TeamRole.Member)
    {
        UserId = userId;
        TeamId = teamId;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}
