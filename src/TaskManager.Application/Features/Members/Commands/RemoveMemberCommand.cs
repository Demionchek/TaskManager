using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;

namespace TaskManager.Application.Features.Members.Commands;

public record RemoveMemberCommand(Guid ProjectId, Guid UserId, Guid MemberId) : IRequest<Result<bool>>;

public class RemoveMemberCommandHandler(IAppDbContext db) : IRequestHandler<RemoveMemberCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveMemberCommand request, CancellationToken ct)
    {
        if (request.MemberId == request.UserId)
            return Result<bool>.Fail(ErrorType.BadRequest, "Cannot remove yourself");
        var project = await db.Projects
                              .Include(p => p.Members)
                              .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null)
            return Result<bool>.Fail(ErrorType.NotFound, "Project not found");
        if (!project.IsOwner(request.UserId))
            return Result<bool>.Fail(ErrorType.Forbidden);
        var member = project.Members.FirstOrDefault(m => m.UserId == request.MemberId);
        if (member == null)
            return Result<bool>.Fail(ErrorType.BadRequest, "User is not a member");
        db.ProjectMembers.Remove(member);
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
