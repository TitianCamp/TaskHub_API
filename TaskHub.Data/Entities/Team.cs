namespace TaskHub.Data.Entities;

public class Team
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    public ICollection<TeamMember> TeamMembers { get; init; } = [];
    public ICollection<TaskItem> Tasks { get; init; } = [];


    public Team()
    { }

    public Team(string name, string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }
}
