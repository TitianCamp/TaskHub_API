using System.ComponentModel.DataAnnotations;
using TaskHub.Data.Entities;

namespace TaskHub.Business.DataModels.TaskItem;

public class TaskCreateRequest
{
    [Required(ErrorMessage = "Task title is required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Task description is required")]
    public string Description { get; set; } = string.Empty;
    [Required(ErrorMessage = "Priority is required")]
    [EnumDataType(typeof(TaskItemPriority), ErrorMessage = "Invalid priority value")]
    public TaskItemPriority Priority { get; set; }
    [Required(ErrorMessage = "Deadline is required")]
    public DateTime Deadline { get; set; }
    [Required(ErrorMessage = "Team ID is required")]
    public Guid TeamId { get; set; }
}
