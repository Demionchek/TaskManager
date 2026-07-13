using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Members.Commands;

public record AddMemberCommand(Guid ProjectId, Guid UserId, Guid NewMemberId, MemberRole Role) : IRequest<Result<ProjectMemberResponse>>;

public class AddMemberCommandHandler(IAppDbContext db) : IRequestHandler<AddMemberCommand, Result<ProjectMemberResponse>>
{
    public async Task<Result<ProjectMemberResponse>> Handle(AddMemberCommand request, CancellationToken ct)
    {
        var project = await db.Projects
                              .Include(p => p.Members)
                              .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct);
        if (project == null)
            return Result<ProjectMemberResponse>.Fail(ErrorType.NotFound);
        if (!project.IsOwner(request.UserId))
            return Result<ProjectMemberResponse>.Fail(ErrorType.Forbidden);
        var userExists = await db.Users.AnyAsync(u => u.Id == request.NewMemberId, ct);
        if (!userExists)
            return Result<ProjectMemberResponse>.Fail(ErrorType.BadRequest, "No user found");
        if (project.Members.Any(m => m.UserId == request.NewMemberId))
            return Result<ProjectMemberResponse>.Fail(ErrorType.BadRequest, "User already a member");
        var member = new ProjectMember
        {
            ProjectId = request.ProjectId,
            UserId = request.NewMemberId,
            Role = request.Role
        };
        db.ProjectMembers.Add(member);
        await db.SaveChangesAsync(ct);
        return Result<ProjectMemberResponse>.Success(new ProjectMemberResponse(member));
    }
}
