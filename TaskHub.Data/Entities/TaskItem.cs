namespace TaskHub.Data.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public TaskItemPriority Priority { get; set; }
    public DateTime Deadline { get; set; }
    public Guid AuthorId { get; set; }
    public Guid TeamId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User Author { get; set; } = null!;
    public Team Team { get; set; } = null!;
    public User? AssignedUser { get; set; }

    public TaskItem()
    { }

    public TaskItem (string title, string description, 
        TaskItemPriority taskPrioritie, DateTime deadline, 
        Guid authorId, Guid teamId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Status = TaskItemStatus.Todo;
        Priority = taskPrioritie;
        Deadline = deadline;
        AuthorId = authorId;
        TeamId = teamId;
        CreatedAt = DateTime.UtcNow;
    }
}