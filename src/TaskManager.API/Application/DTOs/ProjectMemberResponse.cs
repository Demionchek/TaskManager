using TaskManager.API.Domain.Entities;
using TaskManager.API.Domain.Enums;

namespace TaskManager.API.Application.DTOs;

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