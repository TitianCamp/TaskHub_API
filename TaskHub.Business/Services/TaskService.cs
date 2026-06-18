using TaskHub.Business.DataModels.TaskItem;
using TaskHub.Business.Mappers;
using TaskHub.Business.Results;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Business.Services;

public class TaskService
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly ITeamRepository _teamRepository;

    public TaskService(ITaskItemRepository taskRepository, 
        ITeamRepository teamRepository)
    {
        _taskRepository = taskRepository;
        _teamRepository = teamRepository;
    }

    public async Task<Result<TaskResponce>> GetById(
        Guid taskId, Guid userId)
    {
        try
        {
            TaskItem? task = await _taskRepository.GetById(taskId);
            if (task == null)
            {
                return Result.Failure<TaskResponce>("Task not found");
            }

            if (!await IsUserInTeam(userId, task.TeamId))
            {
                return Result.Failure<TaskResponce>("Access denied for this task");
            }

            return Result.Success(task.ToDto());
        }
        catch
        {
            return Result.Failure<TaskResponce>(
                "An error occurred while retrieving the task");
        }
    }

    public async Task<Result<List<TaskResponce>>> GetFilteredTasks(
        Guid userId, Guid teamId, TaskItemStatus? taskStatus, 
        TaskItemPriority? taskPriority)
    {
        try
        {
            if (!await IsUserInTeam(userId, teamId))
            {
                return Result.Failure<List<TaskResponce>>(
                    "You are not a member of this team");
            }

            IEnumerable<TaskItem> taskItems = await _taskRepository
                .GetFilteredTasks(teamId, taskStatus, taskPriority);

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

    public async Task<Result<TaskResponce>> CreateTask(TaskCreateRequest task, Guid userId)
    {
        try
        {
            if (!await IsUserInTeam(userId, task.TeamId))
            {
                return Result.Failure<TaskResponce>(
                    "You are not a member of this team");
            }

            TaskItem taskItem = task.ToEntity(userId);

            await _taskRepository.Add(taskItem);

            return Result.Success(taskItem.ToDto());
        }
        catch
        {
            return Result.Failure<TaskResponce>(
                "An error occurred while creating task");
        }
    }

    private async Task<bool> IsUserInTeam(Guid userId,  Guid teamId)
    {
        return await _teamRepository
            .IsMember(userId, teamId);
    }
}
