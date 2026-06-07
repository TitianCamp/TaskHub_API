using TaskHub.Business.DataModels.TaskItem;
using TaskHub.Data.Entities;

namespace TaskHub.Business.Mappers;

public static class TaskMapper
{
    public static TaskResponce ToDto(this TaskItem task)
    {
        return new TaskResponce
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            Deadline = task.Deadline,
            IsAvailable = task.AssignedUserId == null
        };
    }
}
