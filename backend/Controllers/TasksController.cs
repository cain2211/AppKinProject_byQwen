using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectFlow.DTOs;
using ProjectFlow.Services;

namespace ProjectFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] Guid? projectId, [FromQuery] string? status, [FromQuery] string? priority)
    {
        var tasks = await _taskService.GetAllAsync(projectId, status, priority);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(Guid id)
    {
        var task = await _taskService.GetByIdAsync(id);
        
        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Member")]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto model)
    {
        var result = await _taskService.CreateAsync(model, User.Identity?.Name);
        return CreatedAtAction(nameof(GetTask), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,Member")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto model)
    {
        var result = await _taskService.UpdateAsync(id, model, User.Identity?.Name);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var success = await _taskService.DeleteAsync(id);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id}/move")]
    [Authorize(Roles = "Admin,Manager,Member")]
    public async Task<IActionResult> MoveTask(Guid id, [FromBody] MoveTaskDto model)
    {
        var result = await _taskService.MoveTaskAsync(id, model);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPatch("{id}/dates")]
    [Authorize(Roles = "Admin,Manager,Member")]
    public async Task<IActionResult> UpdateTaskDates(Guid id, [FromBody] UpdateTaskDatesDto model)
    {
        var result = await _taskService.UpdateDatesAsync(id, model, User.Identity?.Name);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("{id}/assign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AssignTask(Guid id, [FromBody] AssignTaskDto model)
    {
        var result = await _taskService.AssignUserAsync(id, model.UserId);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{taskId}/assign/{userId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UnassignTask(Guid taskId, string userId)
    {
        var success = await _taskService.UnassignUserAsync(taskId, userId);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/subtasks")]
    public async Task<IActionResult> GetSubTasks(Guid id)
    {
        var subtasks = await _taskService.GetSubTasksAsync(id);
        return Ok(subtasks);
    }
}
