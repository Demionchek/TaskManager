using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Comments.Queries;

public record GetCommentsQuery(Guid ProjectId, Guid TaskId, Guid UserId) : IRequest<List<CommentResponse>?>;

public class GetCommentsQueryHandler(IAppDbContext db) : IRequestHandler<GetCommentsQuery, List<CommentResponse>?>
{
    public async Task<List<CommentResponse>?> Handle(GetCommentsQuery request, CancellationToken ct)
    {
        var task = await db.Tasks
                           .Include(t => t.Comments)
                           .Include(t => t.Project).ThenInclude(p => p!.Members)
                           .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.ProjectId == request.ProjectId, ct);
        if (task == null || !task.Project!.HasAccess(request.UserId))
            return null;
        return task.Comments.Select(c => new CommentResponse(c)).ToList();
    }
}
