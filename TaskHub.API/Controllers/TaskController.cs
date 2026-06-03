using Microsoft.AspNetCore.Mvc;

namespace TaskHub.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TaskController : ControllerBase
{

    [HttpGet("tasks")]
    public IActionResult Get()
    {
        return Ok("Get all tasks");
    }

    [HttpGet("tasks/{id}")]
    public IActionResult GetById(Guid id)
    {
        return Ok($"Get task with ID: {id}");
    }

    [HttpGet("tasks/search")]
    public IActionResult GetByStatusOrPriority(
        [FromQuery] TaskItemStatus? taskStatus, 
        [FromQuery] TaskItemPriority? taskPriority)
    {
        return Ok($"Get tasks with Status: {taskStatus} or Priority: {taskPriority}");
    }

    [HttpPost("tasks")]
    public IActionResult CreateTask()
    {
        return Ok("Create a new task");
    }

    [HttpPatch("tasks/{id}/take")]
    public IActionResult TakeTask(Guid id)
    {
        return Ok($"Take task with ID: {id}");
    }

    [HttpPatch("tasks/{id}/update")]
    public IActionResult UpdateTask(Guid id)
    {
        return Ok($"Update task with ID: {id}");
    }

    [HttpDelete("tasks/{id}")]
    public IActionResult DeleteTask(Guid id)
    {
        return Ok($"Delete task with ID: {id}");
    }
}
