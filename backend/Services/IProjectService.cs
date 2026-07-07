using ProjectFlow.DTOs;

namespace ProjectFlow.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync(string? status, string? userId, bool includeMembers = false);
    Task<ProjectDto?> GetByIdAsync(Guid id, bool includeTasks = false, bool includeMembers = false);
    Task<ProjectDto> CreateAsync(CreateProjectDto model, string? userId);
    Task<ProjectDto?> UpdateAsync(Guid id, UpdateProjectDto model, string? userId);
    Task<bool> DeleteAsync(Guid id);
    Task<ProjectDto?> AddMemberAsync(Guid projectId, AddProjectMemberDto model);
    Task<bool> RemoveMemberAsync(Guid projectId, string userId);
    Task<GanttTaskDto[]> GetGanttDataAsync(Guid projectId);
}
