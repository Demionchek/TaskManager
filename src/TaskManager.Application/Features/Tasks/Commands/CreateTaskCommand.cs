using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Commands;

public record CreateTaskCommand(Guid ProjectId, Guid UserId, string Title, string Description, PriorityEnum Priority, Guid? AssigneeId) : IRequest<TaskResponse?>;

public class CreateTaskCommandHandler(IAppDbContext db) : IRequestHandler<CreateTaskCommand, TaskResponse?>
{
    public async Task<TaskResponse?> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        var project = await db.Projects
                              .Include(p => p.Members)
                              .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null || !project.HasAccess(request.UserId)) return null;
        TaskItem taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            AssigneeId = request.AssigneeId,
            Status = StatusEnum.Todo,
            CreatedAt = DateTime.UtcNow,
            ProjectId = request.ProjectId
        };
        await db.Tasks.AddAsync(taskItem, ct);
        await db.SaveChangesAsync(ct);
        return new TaskResponse(taskItem);
    }
}