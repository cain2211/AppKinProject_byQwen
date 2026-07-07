using Microsoft.AspNetCore.Identity;

namespace ProjectFlow.Models;

public class Task
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "NotStarted";
    public string Priority { get; set; } = "Medium";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    public int Order { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? ParentTaskId { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navegación
    public virtual Project? Project { get; set; }
    public virtual Task? ParentTask { get; set; }
    public virtual ICollection<Task> SubTasks { get; set; } = new List<Task>();
    public virtual ICollection<TaskAssignment> Assignments { get; set; } = new List<TaskAssignment>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
