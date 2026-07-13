using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;

namespace TaskManager.Application.Features.Tasks.Commands;

public record DeleteTaskCommand(Guid ProjectId, Guid TaskId, Guid UserId) : IRequest<bool>;

public class DeleteTaskCommandHandler(IAppDbContext db) : IRequestHandler<DeleteTaskCommand, bool>
{
    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken ct)
    {
        var project = await db.Projects
                              .Include(p => p.Members)
                              .Include(p => p.Tasks)
                              .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null || !project.HasAccess(request.UserId))
            return false;
        var task = project.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task == null) return false;
        db.Tasks.Remove(task);
        await db.SaveChangesAsync(ct);
        return true;
    }
}