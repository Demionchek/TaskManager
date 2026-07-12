using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Projects.Queries;

public record GetProjectsQuery(Guid UserId) : IRequest<List<ProjectResponse>>;

public class GetProjectsQueryHandler(IAppDbContext db)
    : IRequestHandler<GetProjectsQuery, List<ProjectResponse>>
{
    public async Task<List<ProjectResponse>> Handle(GetProjectsQuery request, CancellationToken ct)
    {
        var projects = await db.Projects
                               .Where(p => p.OwnerId == request.UserId)
                               .ToListAsync(ct);
        return projects.Select(p => new ProjectResponse(p)).ToList();
    }
}