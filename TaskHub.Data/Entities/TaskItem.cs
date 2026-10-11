namespace TaskHub.Data.Entities;

public class TaskItem
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TaskItemStatus Status { get; private set; }
    public TaskItemPriority Priority { get; init; }
    public DateTime Deadline { get; init; }
    public Guid AuthorId { get; init; }
    public Guid TeamId { get; init; }
    public Guid? AssignedUserId { get; private set; }
    public DateTime CreatedAt { get; init; }

    public User Author { get; init; } = null!;
    public Team Team { get; init; } = null!;
    public User? AssignedUser { get; private set; }

    public TaskItem()
    { }

    public TaskItem (string title, string description, 
        TaskItemPriority taskPriority, DateTime deadline, 
        Guid authorId, Guid teamId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Status = TaskItemStatus.Todo;
        Priority = taskPriority;
        Deadline = deadline;
        AuthorId = authorId;
        TeamId = teamId;
        CreatedAt = DateTime.UtcNow;
    }

    public void AssignTask(Guid? user)
    {
        AssignedUserId =  user;
    }
    
    public void ChangeStatus(TaskItemStatus status)
    {
        if (Status == TaskItemStatus.Done)
        {
            return;
        }

        if (status == TaskItemStatus.Done
            && Status != TaskItemStatus.InProgress)
        {
            return;
        }
        
        Status = status;
    }
}