namespace TaskHub.Business.DataModels.TaskItem;

public class TaskResponce
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public bool IsAvailable { get; set; }
}
