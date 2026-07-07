namespace ProjectFlow.Models;

public class TaskAssignment
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    
    // Navegación
    public virtual Task? Task { get; set; }
}
