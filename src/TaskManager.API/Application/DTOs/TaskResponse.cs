using TaskManager.API.Domain.Entities;
using TaskManager.API.Domain.Enums;

namespace TaskManager.API.Application.DTOs;

public class TaskResponse
{
    public TaskResponse(TaskItem taskItem)
    {
        Id = taskItem.Id;
        Title = taskItem.Title;
        Description = taskItem.Description;
        CreatedAt = taskItem.CreatedAt;
        Status = taskItem.Status;
        Priority = taskItem.Priority;
        ProjectId = taskItem.ProjectId;
        AssigneeId = taskItem.AssigneeId;
    }

    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssigneeId { get; set; }
    public StatusEnum Status { get; set; }
    public PriorityEnum Priority { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}