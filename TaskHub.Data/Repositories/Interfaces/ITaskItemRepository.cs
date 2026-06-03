using TaskHub.Data.Entities;

namespace TaskHub.Data.Repositories.Interfaces;

public interface ITaskItemRepository 
    : IRepository<TaskItem>
{
    Task<IEnumerable<TaskItem>> GetByStatusAndPriority(
        TaskItemStatus? taskStatus, TaskItemPriority? taskPriority);
}