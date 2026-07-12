using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Projects.Commands;

public record UpdateProjectCommand(Guid Id, Guid UserId, string Name, string? Description) :  IRequest<ProjectResponse?>;

public class UpdateProjectCommandHandler(IAppDbContext db)
    : IRequestHandler<UpdateProjectCommand, ProjectResponse?>
{
    public async Task<ProjectResponse?> Handle(UpdateProjectCommand request, CancellationToken ct)
    {
        var project = await db.Projects.Where(p => p.Id == request.Id).SingleOrDefaultAsync(ct);
        if (project == null || project.OwnerId != request.UserId) return null;
        project.Name = request.Name;
        project.Description = request.Description;
        await db.SaveChangesAsync(ct);
        return new ProjectResponse(project);

    }
}