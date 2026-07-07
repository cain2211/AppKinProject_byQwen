using ProjectFlow.DTOs;

namespace ProjectFlow.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync(Guid? projectId, string? status, string? priority);
    Task<TaskDto?> GetByIdAsync(Guid id);
    Task<TaskDto> CreateAsync(CreateTaskDto model, string? userId);
    Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskDto model, string? userId);
    Task<bool> DeleteAsync(Guid id);
    Task<TaskDto?> MoveTaskAsync(Guid id, MoveTaskDto model);
    Task<TaskDto?> UpdateDatesAsync(Guid id, UpdateTaskDatesDto model, string? userId);
    Task<TaskDto?> AssignUserAsync(Guid id, string userId);
    Task<bool> UnassignUserAsync(Guid taskId, string userId);
    Task<IEnumerable<TaskDto>> GetSubTasksAsync(Guid parentId);
}
