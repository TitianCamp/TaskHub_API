using System.Threading.Tasks;
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

    public async Task<Result<TaskResponse>> GetById(
        Guid taskId, Guid userId)
    {
        try
        {
            TaskItem? task = await _taskRepository.GetById(taskId);
            if (task == null)
            {
                return Result.Failure<TaskResponse>("Task not found");
            }

            if (!await IsUserInTeam(userId, task.TeamId))
            {
                return Result.Failure<TaskResponse>("Access denied for this task");
            }

            return Result.Success(task.ToDto());
        }
        catch
        {
            return Result.Failure<TaskResponse>(
                "An error occurred while retrieving the task");
        }
    }

    public async Task<Result<List<TaskResponse>>> GetFilteredTasks(
        Guid userId, Guid teamId, TaskItemStatus? taskStatus,
        TaskItemPriority? taskPriority)
    {
        try
        {
            if (!await IsUserInTeam(userId, teamId))
            {
                return Result.Failure<List<TaskResponse>>(
                    "You are not a member of this team");
            }

            IEnumerable<TaskItem> taskItems = await _taskRepository
                .GetFilteredTasks(teamId, taskStatus, taskPriority);

            if (taskItems == null || !taskItems.Any())
            {
                return Result.Failure<List<TaskResponse>>("No tasks found");
            }

            List<TaskResponse> tasks = taskItems
                .Select(ti => ti.ToDto()).ToList();

            return Result.Success(tasks);
        }
        catch
        {
            return Result.Failure<List<TaskResponse>>(
                "An error occurred while retrieving tasks");
        }
    }

    public async Task<Result<TaskResponse>> CreateTask(TaskCreateRequest task, Guid userId)
    {
        try
        {
            if (!await IsUserInTeam(userId, task.TeamId))
            {
                return Result.Failure<TaskResponse>(
                    "You are not a member of this team");
            }

            TaskItem taskItem = task.ToEntity(userId);

            await _taskRepository.Add(taskItem);

            return Result.Success(taskItem.ToDto());
        }
        catch
        {
            return Result.Failure<TaskResponse>(
                "An error occurred while creating task");
        }
    }

    public async Task<Result<TaskResponse>> TakeTask(Guid id, Guid userId)
    {
        try
        {
            var task = await _taskRepository.GetById(id);

            if (task == null)
            {
                return Result.Failure<TaskResponse>("Task not found");
            }

            if (!await IsUserInTeam(userId, task.TeamId))
            {
                return Result.Failure<TaskResponse>(
                    "You do not have access to this team's tasks");
            }

            if (task.AssignedUserId != null)
            {
                return Result.Failure<TaskResponse>(
                    "This task is already assigned to another user");
            }

            task.AssignedUserId = userId;
            task.Status = TaskItemStatus.InProgress;

            await _taskRepository.Update(task);

            return Result.Success(task.ToDto());
        }
        catch
        {
            return Result.Failure<TaskResponse>(
                "An error occurred while creating task");
        }
    }

    public async Task<Result<TaskResponse>> FinishTask(Guid id, Guid userId)
    {
        try
        {
            var task = await _taskRepository.GetById(id);

            if (task == null)
            {
                return Result.Failure<TaskResponse>("Task not found");
            }

            if (task.AssignedUserId != userId)
            {
                return Result.Failure<TaskResponse>("Only the assigned user can finish this task");
            }

            if (task.Status == TaskItemStatus.Done)
            {
                return Result.Failure<TaskResponse>("Task is already finished");
            }

            task.Status |= TaskItemStatus.Done;

            await _taskRepository.Update(task);

            return Result.Success(task.ToDto());
        }
        catch
        {
            return Result.Failure<TaskResponse>(
                "An error occurred while creating task");
        }
    }

    public async Task<Result<string>> DeleteTask(Guid id, Guid userId)
    {
        try
        {
            var task = await _taskRepository.GetById(id);
            if (task == null)
            {
                return Result.Failure<string>("Task not found");
            }

            if (task.AuthorId != userId || 
                !await IsUserTeamOwner(userId, task.TeamId))
            {
                return Result.Failure<string>(
                    "Only the author or team owner can delete tasks");
            }

            await _taskRepository.Delete(id);

            return Result.Success("Task was deleted");
        }
        catch
        {
            return Result.Failure<string>(
               "An error occurred while creating task");
        }
    }

    private async Task<bool> IsUserInTeam(Guid userId, Guid teamId)
    {
        return await _teamRepository
            .IsMember(userId, teamId);
    }

    private async Task<bool> IsUserTeamOwner(Guid userId, Guid teamId)
    {
        return await _teamRepository
            .IsOwner(userId, teamId);
    }
}
