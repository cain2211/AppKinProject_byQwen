namespace ProjectFlow.Models;

public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
    public DateTime JoinedDate { get; set; }
    
    // Navegación
    public virtual Project? Project { get; set; }
}
