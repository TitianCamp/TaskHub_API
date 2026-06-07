using Microsoft.AspNetCore.Mvc;

namespace TaskHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _taskService.GetAllTasks();

        if (result.IsSuccess)
        {
            return Ok(result.Value!);
        }
        else
        {
            return BadRequest(result.Error!);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _taskService.GetById(id);

        if (result.IsSuccess)
        {
            return Ok(result.Value!);
        }
        else
        {
            return BadRequest(result.Error!);
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetByStatusOrPriority(
        [FromQuery] TaskItemStatus? taskStatus,
        [FromQuery] TaskItemPriority? taskPriority)
    {
        var result = await _taskService
            .GetByStatusOrPriority(taskStatus, taskPriority);

        if (result.IsSuccess)
        {
            return Ok(result.Value!);
        }
        else
        {
            return BadRequest(result.Error!);
        }
    }

    [HttpPost]
    public IActionResult CreateTask()
    {
        return Ok("Create a new task");
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
