using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectFlow.DTOs;
using ProjectFlow.Services;

namespace ProjectFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects([FromQuery] string? status, [FromQuery] bool? includeMembers)
    {
        var projects = await _projectService.GetAllAsync(status, User.Identity?.Name, includeMembers ?? false);
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(Guid id, [FromQuery] bool includeTasks, [FromQuery] bool includeMembers)
    {
        var project = await _projectService.GetByIdAsync(id, includeTasks, includeMembers);
        
        if (project == null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto model)
    {
        var result = await _projectService.CreateAsync(model, User.Identity?.Name);
        return CreatedAtAction(nameof(GetProject), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDto model)
    {
        var result = await _projectService.UpdateAsync(id, model, User.Identity?.Name);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var success = await _projectService.DeleteAsync(id);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/members")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddProjectMemberDto model)
    {
        var result = await _projectService.AddMemberAsync(id, model);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{projectId}/members/{userId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RemoveMember(Guid projectId, string userId)
    {
        var success = await _projectService.RemoveMemberAsync(projectId, userId);
        
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/gantt-data")]
    public async Task<IActionResult> GetGanttData(Guid id)
    {
        var data = await _projectService.GetGanttDataAsync(id);
        return Ok(data);
    }
}
