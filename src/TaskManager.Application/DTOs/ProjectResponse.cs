using TaskManager.Domain.Entities;

namespace TaskManager.Application.DTOs;

public class ProjectResponse
{
    public ProjectResponse(Project? project)
    {
        if (project != null)
        {
            Id = project.Id;
            Name = project.Name;
            Description = project.Description;
            CreatedAt = project.CreatedAt;
            OwnerId = project.OwnerId;
        }
    }

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid OwnerId { get; set; }
}