using Microsoft.EntityFrameworkCore;
using TaskManager.API.Domain.Enums;

namespace TaskManager.API.Domain.Entities;

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public MemberRole Role { get; set; }
}