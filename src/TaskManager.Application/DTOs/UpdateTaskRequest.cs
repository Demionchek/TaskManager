using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public class UpdateTaskRequest
{
    public Guid? AssigneeId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public StatusEnum Status { get; set; }
    public PriorityEnum Priority { get; set; }
}