using TaskHub.Business.DataModels.TaskItem;
using TaskHub.Business.Mappers;
using TaskHub.Business.Results;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Business.Services;

public class TaskService
{
    private readonly ITaskItemRepository _taskRepository;

    public TaskService(ITaskItemRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<List<TaskResponce>>> GetAllTasks()
    {
        try
        {
            IEnumerable<TaskItem> taskItems = await _taskRepository.GetAll();

            if (taskItems == null || !taskItems.Any())
            {
                return Result.Failure<List<TaskResponce>>("No tasks found");
            }

            List<TaskResponce> tasks = taskItems
                .Select(ti => ti.ToDto()).ToList();

            return Result.Success(tasks);
        }
        catch
        {
            return Result.Failure<List<TaskResponce>>(
                "An error occurred while retrieving tasks");
        }
    }

    public async Task<Result<TaskResponce>> GetById(Guid id)
    {
        try
        {
            TaskItem? task = await _taskRepository.GetById(id);
            if (task == null)
            {
                return Result.Failure<TaskResponce>("Task not found");
            }

            return Result.Success(task.ToDto());
        }
        catch
        {
            return Result.Failure<TaskResponce>(
                "An error occurred while retrieving the task");
        }
    }

    public async Task<Result<List<TaskResponce>>> GetByStatusOrPriority(
        TaskItemStatus? taskStatus, TaskItemPriority? taskPriority)
    {
        try
        {
            IEnumerable<TaskItem> taskItems = await _taskRepository
                .GetByStatusAndPriority(taskStatus, taskPriority);

            if (taskItems == null ||  !taskItems.Any())
            {
                return Result.Failure<List<TaskResponce>>("No tasks found");
            }

            List<TaskResponce> tasks = taskItems
                .Select(ti => ti.ToDto()).ToList();

            return Result.Success(tasks);
        }
        catch
        {
            return Result.Failure<List<TaskResponce>>(
                "An error occurred while retrieving tasks");
        }
    }
}
