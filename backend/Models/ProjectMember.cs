namespace ProjectFlow.Models;

public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public MemberRole Role { get; set; }
    public DateTime JoinedDate { get; set; }
    
    // Navegación
    public virtual Project? Project { get; set; }
}

public enum MemberRole
{
    Viewer,
    Member,
    Manager,
    Admin
}
