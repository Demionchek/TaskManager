using TaskManager.API.Application.DTOs;
using TaskManager.API.Domain.Enums;

namespace TaskManager.API.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public StatusEnum Status { get; set; }
    public PriorityEnum Priority { get; set; }
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }
    public Guid? AssigneeId { get; set; }
    public User? Assignee { get; set; }
    public List<Comment?> Comments { get; set; } = [];
    public List<ProjectMember> Members { get; set; } = [];
}