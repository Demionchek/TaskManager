using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Members.Queries;

public record GetMembersQuery(Guid ProjectId, Guid UserId) : IRequest<Result<List<ProjectMemberResponse>>>;

public class GetMembersQueryHandler(IAppDbContext db) : IRequestHandler<GetMembersQuery, Result<List<ProjectMemberResponse>>>
{
    public async Task<Result<List<ProjectMemberResponse>>> Handle(GetMembersQuery request, CancellationToken ct)
    {
        var project = await db.Projects
                              .Include(p => p.Members)
                              .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null)
            return Result<List<ProjectMemberResponse>>.Fail(ErrorType.NotFound);
        if (!project.IsOwner(request.UserId))
            return Result<List<ProjectMemberResponse>>.Fail(ErrorType.Forbidden);
        var members = project.Members.Select(m => new ProjectMemberResponse(m)).ToList();
        return Result<List<ProjectMemberResponse>>.Success(members);
    }
}
