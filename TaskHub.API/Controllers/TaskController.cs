using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
    public IActionResult TakeTask(Guid id)
    {
        return Ok($"Take task with ID: {id}");
    }

    [HttpPatch("{id}/update")]
    public IActionResult UpdateTask(Guid id)
    {
        return Ok($"Update task with ID: {id}");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTask(Guid id)
    {
        return Ok($"Delete task with ID: {id}");
    }
}
