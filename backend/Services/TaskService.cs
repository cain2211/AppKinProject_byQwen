using Microsoft.EntityFrameworkCore;
using ProjectFlow.Data;
using ProjectFlow.DTOs;
using ProjectFlow.Models;

namespace ProjectFlow.Services;

public class TaskService : ITaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskDto>> GetAllAsync(Guid? projectId, string? status, string? priority)
    {
        var query = _context.Tasks.AsQueryable();

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(t => t.Status == status);
        if (!string.IsNullOrEmpty(priority))
            query = query.Where(t => t.Priority == priority);

        var tasks = await query.ToListAsync();

        return tasks.Select(t => MapToDto(t));
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return null;

        return MapToDto(task);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskDto model, string? userId)
    {
        var task = new Models.Task
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Description = model.Description,
            Priority = model.Priority,
            Status = "NotStarted",
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            EstimatedHours = model.EstimatedHours,
            ProjectId = model.ProjectId,
            ParentTaskId = model.ParentTaskId,
            Order = 0,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = userId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Asignar usuarios si se proporcionaron
        if (model.AssignedUserIds != null && model.AssignedUserIds.Any())
        {
            foreach (var userIdItem in model.AssignedUserIds)
            {
                var assignment = new TaskAssignment
                {
                    Id = Guid.NewGuid(),
                    TaskId = task.Id,
                    UserId = userIdItem,
                    AssignedDate = DateTime.UtcNow
                };
                _context.TaskAssignments.Add(assignment);
            }
            await _context.SaveChangesAsync();
        }

        return MapToDto(task);
    }

    public async Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskDto model, string? userId)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return null;

        if (!string.IsNullOrEmpty(model.Title)) task.Title = model.Title;
        if (!string.IsNullOrEmpty(model.Description)) task.Description = model.Description;
        if (!string.IsNullOrEmpty(model.Status)) task.Status = model.Status;
        if (!string.IsNullOrEmpty(model.Priority)) task.Priority = model.Priority;
        if (model.StartDate.HasValue) task.StartDate = model.StartDate.Value;
        if (model.EndDate.HasValue) task.EndDate = model.EndDate.Value;
        if (model.EstimatedHours.HasValue) task.EstimatedHours = model.EstimatedHours.Value;
        if (model.ActualHours.HasValue) task.ActualHours = model.ActualHours.Value;

        await _context.SaveChangesAsync();

        return MapToDto(task);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TaskDto?> MoveTaskAsync(Guid id, MoveTaskDto model)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return null;

        task.Status = model.Status;
        task.Order = model.Order;

        if (model.NewProjectId.HasValue)
            task.ProjectId = model.NewProjectId.Value;

        await _context.SaveChangesAsync();
        return MapToDto(task);
    }

    public async Task<TaskDto?> UpdateDatesAsync(Guid id, UpdateTaskDatesDto model, string? userId)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return null;

        task.StartDate = model.StartDate;
        task.EndDate = model.EndDate;

        await _context.SaveChangesAsync();
        return MapToDto(task);
    }

    public async Task<TaskDto?> AssignUserAsync(Guid id, string userId)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return null;

        var existingAssignment = await _context.TaskAssignments
            .FirstOrDefaultAsync(a => a.TaskId == id && a.UserId == userId);

        if (existingAssignment != null)
            return MapToDto(task);

        var assignment = new TaskAssignment
        {
            Id = Guid.NewGuid(),
            TaskId = id,
            UserId = userId,
            AssignedDate = DateTime.UtcNow
        };

        _context.TaskAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        return MapToDto(task);
    }

    public async Task<bool> UnassignUserAsync(Guid taskId, string userId)
    {
        var assignment = await _context.TaskAssignments
            .FirstOrDefaultAsync(a => a.TaskId == taskId && a.UserId == userId);

        if (assignment == null) return false;

        _context.TaskAssignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TaskDto>> GetSubTasksAsync(Guid parentId)
    {
        var subtasks = await _context.Tasks
            .Where(t => t.ParentTaskId == parentId)
            .ToListAsync();

        return subtasks.Select(MapToDto);
    }

    private TaskDto MapToDto(Models.Task task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            Order = task.Order,
            ProjectId = task.ProjectId,
            ParentTaskId = task.ParentTaskId,
            CreatedBy = task.CreatedBy,
            CreatedDate = task.CreatedDate,
            AssignedUsers = task.Assignments?.Select(a => a.UserId).ToList(),
            SubTasksCount = task.SubTasks?.Count ?? 0,
            CommentsCount = 0 // Se puede implementar conteo real de comentarios
        };
    }
}
