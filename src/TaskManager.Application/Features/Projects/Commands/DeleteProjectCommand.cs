using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;

namespace TaskManager.Application.Features.Projects.Commands;

public record DeleteProjectCommand(Guid Id, Guid UserId) : IRequest<bool>;

public class DeleteProjectCommandHandler(IAppDbContext db) : IRequestHandler<DeleteProjectCommand, bool>
{
    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken ct)
    {
        var project = await db.Projects.Where(p => p.Id == request.Id).SingleOrDefaultAsync(ct);
        if (project == null || project.OwnerId != request.UserId) return false;
        db.Projects.Remove(project);
        await db.SaveChangesAsync(ct);
        return true;
    }
}