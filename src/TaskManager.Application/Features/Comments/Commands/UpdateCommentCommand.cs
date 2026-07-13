using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Features.Comments.Commands;

public record UpdateCommentCommand(Guid ProjectId, Guid TaskId, Guid CommentId, Guid UserId, string Content) : IRequest<CommentResponse?>;

public class UpdateCommentCommandHandler(IAppDbContext db) : IRequestHandler<UpdateCommentCommand, CommentResponse?>
{
    public async Task<CommentResponse?> Handle(UpdateCommentCommand request, CancellationToken ct)
    {
        var task = await db.Tasks
                           .Include(t => t.Project).ThenInclude(p => p!.Members)
                           .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.ProjectId == request.ProjectId, ct);
        if (task == null || !task.Project!.HasAccess(request.UserId))
            return null;
        var comment = await db.Comments
                              .FirstOrDefaultAsync(c => c.Id == request.CommentId && c.TaskItemId == request.TaskId, ct);
        if (comment == null || !comment.IsAuthor(request.UserId))
            return null;
        comment.Content = request.Content;
        await db.SaveChangesAsync(ct);
        return new CommentResponse(comment);
    }
}
