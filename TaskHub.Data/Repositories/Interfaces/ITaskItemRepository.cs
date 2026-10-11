using TaskHub.Data.Entities;

namespace TaskHub.Data.Repositories.Interfaces;

public interface ITaskItemRepository 
    : IRepository<TaskItem>
{
    Task<IReadOnlyList<TaskItem>> GetFilteredTasks(Guid teamId,
        TaskItemStatus? taskStatus, TaskItemPriority? taskPriority);
    Task<IReadOnlyList<TaskItem>> GetForTeam(Guid teamId);
}