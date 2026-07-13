using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Tasks.Queries;

public record GetTasksQuery(Guid ProjectId, Guid UserId) : IRequest<List<TaskResponse>?>;

public class GetTasksQueryHandler(IAppDbContext db) : IRequestHandler<GetTasksQuery, List<TaskResponse>?>
{
    public async Task<List<TaskResponse>?> Handle(GetTasksQuery request, CancellationToken ct)
    {
        var project = await db.Projects
                              .Include(p => p.Tasks).ThenInclude(t => t.Assignee)
                              .Include(p => p.Members)
                              .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null || !project.HasAccess(request.UserId))
            return null;
        return project.Tasks.Select(t => new TaskResponse(t)).ToList();
    }
}