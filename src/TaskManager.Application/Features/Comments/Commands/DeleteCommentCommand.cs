using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;

namespace TaskManager.Application.Features.Comments.Commands;

public record DeleteCommentCommand(Guid ProjectId, Guid TaskId, Guid CommentId, Guid UserId) : IRequest<bool>;

public class DeleteCommentCommandHandler(IAppDbContext db) : IRequestHandler<DeleteCommentCommand, bool>
{
    public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken ct)
    {
        var task = await db.Tasks
                           .Include(t => t.Project).ThenInclude(p => p!.Members)
                           .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.ProjectId == request.ProjectId, ct);
        if (task == null || !task.Project!.HasAccess(request.UserId))
            return false;
        var comment = await db.Comments
                              .FirstOrDefaultAsync(c => c.Id == request.CommentId && c.TaskItemId == request.TaskId, ct);
        if (comment == null || !comment.IsAuthor(request.UserId))
            return false;
        db.Comments.Remove(comment);
        await db.SaveChangesAsync(ct);
        return true;
    }
}
