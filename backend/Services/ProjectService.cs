using Microsoft.EntityFrameworkCore;
using ProjectFlow.Data;
using ProjectFlow.DTOs;
using ProjectFlow.Models;
using TaskModel = ProjectFlow.Models.Task;

namespace ProjectFlow.Services;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync(string? status, string? userId, bool includeMembers = false)
    {
        var query = _context.Projects.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);

        // Si es usuario específico, filtrar proyectos donde es miembro
        if (!string.IsNullOrEmpty(userId))
        {
            var memberProjectIds = await _context.ProjectMembers
                .Where(pm => pm.UserId == userId)
                .Select(pm => pm.ProjectId)
                .ToListAsync();

            query = query.Where(p => memberProjectIds.Contains(p.Id));
        }

        var projects = await query.ToListAsync();

        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Status = p.Status,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            CreatedDate = p.CreatedDate,
            CreatedBy = p.CreatedBy,
            TasksCount = p.Tasks?.Count ?? 0,
            CompletedTasksCount = p.Tasks?.Count(t => t.Status == "Completed") ?? 0
        });
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, bool includeTasks = false, bool includeMembers = false)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return null;

        var dto = new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            CreatedDate = project.CreatedDate,
            CreatedBy = project.CreatedBy,
            TasksCount = project.Tasks?.Count ?? 0,
            CompletedTasksCount = project.Tasks?.Count(t => t.Status == "Completed") ?? 0
        };

        return dto;
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto model, string? userId)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Status = "Planning",
            CreatedDate = DateTime.UtcNow,
            CreatedBy = userId
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            CreatedDate = project.CreatedDate,
            CreatedBy = project.CreatedBy,
            TasksCount = 0,
            CompletedTasksCount = 0
        };
    }

    public async Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectDto model, string? userId)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return null;

        if (!string.IsNullOrEmpty(model.Name)) project.Name = model.Name;
        if (!string.IsNullOrEmpty(model.Description)) project.Description = model.Description;
        if (!string.IsNullOrEmpty(model.Status)) project.Status = model.Status;
        if (model.StartDate.HasValue) project.StartDate = model.StartDate.Value;
        if (model.EndDate.HasValue) project.EndDate = model.EndDate.Value;

        await _context.SaveChangesAsync();

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            CreatedDate = project.CreatedDate,
            CreatedBy = project.CreatedBy,
            TasksCount = project.Tasks?.Count ?? 0,
            CompletedTasksCount = project.Tasks?.Count(t => t.Status == "Completed") ?? 0
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return false;

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProjectDto?> AddMemberAsync(Guid projectId, AddProjectMemberDto model)
    {
        var project = await _context.Projects.FindAsync(projectId);
        if (project == null) return null;

        var member = new ProjectMember
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            UserId = model.UserId,
            Role = model.Role,
            JoinedDate = DateTime.UtcNow
        };

        _context.ProjectMembers.Add(member);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(projectId);
    }

    public async Task<bool> RemoveMemberAsync(Guid projectId, string userId)
    {
        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
        
        if (member == null) return false;

        _context.ProjectMembers.Remove(member);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<GanttTaskDto[]> GetGanttDataAsync(Guid projectId)
    {
        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

        return tasks.Select(t => new GanttTaskDto
        {
            Id = t.Id,
            Title = t.Title,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            Status = t.Status,
            Priority = t.Priority,
            ParentTaskId = t.ParentTaskId,
            Progress = CalculateProgress(t),
            AssignedUsers = t.Assignments?.Select(a => a.UserId).ToList()
        }).ToArray();
    }

    private int CalculateProgress(Models.Task task)
    {
        if (task.Status == "Completed") return 100;
        if (task.Status == "NotStarted") return 0;
        return 50; // Default para InProgress
    }
}
