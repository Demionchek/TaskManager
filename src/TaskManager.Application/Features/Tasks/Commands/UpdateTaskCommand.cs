using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Commands;

public record UpdateTaskCommand(Guid ProjectId, Guid UserId, Guid TaskId, string Title, string Description, PriorityEnum Priority, Guid? AssigneeId, StatusEnum Status) : IRequest<TaskResponse?>;

public class UpdateTaskCommandHandler(IAppDbContext db) : IRequestHandler<UpdateTaskCommand, TaskResponse?>
{
    public async Task<TaskResponse?> Handle(UpdateTaskCommand request, CancellationToken ct)
    {
        var project = await db.Projects
                              .Include(p => p.Tasks).ThenInclude(t => t.Assignee)
                              .Include(p => p.Members)
                              .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null || !project.HasAccess(request.UserId))
            return null;
        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null) return null;
        MapUpdateTask(request, task);
        await db.SaveChangesAsync(ct);
        return new TaskResponse(task);
    }

    private static void MapUpdateTask(UpdateTaskCommand request, TaskItem task)
    {
        task.AssigneeId = request.AssigneeId;
        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;
    }
}