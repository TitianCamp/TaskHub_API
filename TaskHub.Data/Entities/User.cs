using Microsoft.AspNetCore.Identity;

namespace TaskHub.Data.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    public ICollection<RefreshToken> RefreshTokens { get; init; } = [];
    public ICollection<TeamMember> TeamMemberships { get; init; } = [];
    public ICollection<TaskItem> AuthoredTasks { get; init; } = [];
    public ICollection<TaskItem> AssignedTasks { get; init; } = [];

    public User()
    {
    }

    public User(
        string firstName,
        string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        CreatedAt = DateTime.UtcNow;
    }
}