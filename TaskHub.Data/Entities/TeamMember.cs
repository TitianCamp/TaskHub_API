namespace TaskHub.Data.Entities;

public class TeamMember
{
    public Guid UserId { get; set; }
    public Guid TeamId { get; set; }
    public TeamRole Role { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Team Team { get; set; } = null!;

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
