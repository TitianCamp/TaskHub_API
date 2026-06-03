namespace TaskHub.Data.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User Owner { get; set; } = null!;
    public ICollection<TeamMember> TeamMembers { get; set; } = [];

    public Team()
    { }

    public Team(string name, string description, Guid ownerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        OwnerId = ownerId;
        CreatedAt = DateTime.UtcNow;
    }
}
