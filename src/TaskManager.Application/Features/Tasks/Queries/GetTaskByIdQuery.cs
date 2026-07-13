using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Tasks.Queries;

public record GetTaskByIdQuery(Guid ProjectId, Guid TaskId, Guid UserId) : IRequest<TaskResponse?>;

public class GetTaskByIdQueryHandler(IAppDbContext db) : IRequestHandler<GetTaskByIdQuery, TaskResponse?>
{
    public async Task<TaskResponse?> Handle(GetTaskByIdQuery request, CancellationToken ct)
    {
        var project = await db.Projects
                              .Include(p => p.Tasks).ThenInclude(t => t.Assignee)
                              .Include(p => p.Members)
                              .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null || !project.HasAccess(request.UserId))
            return null;
        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null)  return null;
        return new TaskResponse(task);
    }
}