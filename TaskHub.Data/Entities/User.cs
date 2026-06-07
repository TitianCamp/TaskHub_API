using Microsoft.AspNetCore.Identity;

namespace TaskHub.Data.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<TeamMember> TeamMemberships { get; set; } = [];
    public ICollection<TaskItem> AuthoredTasks { get; set; } = [];
    public ICollection<TaskItem> AssignedTasks { get; set; } = [];

    public User()
    { }

    public User(
        string userName, string firstName, 
        string lastName, string email)
    {
        Id = Guid.NewGuid();
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }
}
