using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public class ProjectMemberResponse
{
    public ProjectMemberResponse(ProjectMember projectMember)
    {
        ProjectId = projectMember.ProjectId;
        UserId = projectMember.UserId;
        Role = projectMember.Role;
    }

    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public MemberRole Role { get; set; }
}