using Microsoft.AspNetCore.Mvc;
using TaskHub.Business.DataModels.TaskItem;

namespace TaskHub.API.Controllers;

public class TaskController : BaseApiController
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("{teamId}/tasks")]
    public async Task<IActionResult> GetByStatusOrPriority(
        Guid teamId,
        [FromQuery] TaskItemStatus? taskStatus,
        [FromQuery] TaskItemPriority? taskPriority)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _taskService
            .GetFilteredTasks(userId.Value, teamId, taskStatus, taskPriority);

        return SendResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _taskService.GetById(id, userId.Value);

        return SendResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(
        [FromBody] TaskCreateRequest request)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _taskService.CreateTask(request, userId.Value);

        return SendResult(result);
    }

    [HttpPatch("{id}/take")]
    public async Task<IActionResult> TakeTask(Guid id)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _taskService.TakeTask(id, userId.Value);

        return SendResult(result);
    }

    [HttpPatch("{id}/finish")]
    public async Task<IActionResult> FinishTask(Guid id)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _taskService.FinishTask(id, userId.Value);

        return SendResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        Guid? userId = GetCurrentUser();
        if (userId == null)
        {
            return Unauthorized("User ID not found in token.");
        }

        var result = await _taskService.DeleteTask(id, userId.Value);

        return SendResult(result);
    }
}
