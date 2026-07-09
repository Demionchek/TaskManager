using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public MemberRole Role { get; set; }
}