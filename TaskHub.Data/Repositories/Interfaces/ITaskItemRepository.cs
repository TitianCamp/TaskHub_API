using TaskHub.Data.Entities;

namespace TaskHub.Data.Repositories.Interfaces;

public interface ITaskItemRepository 
    : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetFilteredTasks(Guid teamId,
        TaskItemStatus? taskStatus, TaskItemPriority? taskPriority);
    Task<IEnumerable<TaskItem>> GetForTeam(Guid teamId);
}