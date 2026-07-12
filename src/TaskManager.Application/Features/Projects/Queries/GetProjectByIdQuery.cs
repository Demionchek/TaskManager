using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Projects.Queries;

public record GetProjectByIdQuery(Guid Id, Guid UserId) : IRequest<ProjectResponse?>;

public class GetProjectByIdQueryHandler(IAppDbContext db)
    : IRequestHandler<GetProjectByIdQuery, ProjectResponse?>
{
    public async Task<ProjectResponse?> Handle(GetProjectByIdQuery request, CancellationToken ct)
    {
        var project = await db.Projects.Where(p => p.Id == request.Id && p.OwnerId == request.UserId)
                              .FirstOrDefaultAsync(ct);
        if (project == null) return null;
        return new ProjectResponse(project);
    }
}