using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Comments.Commands;

public record CreateCommentCommand(Guid ProjectId, Guid TaskId, Guid UserId, string Content) : IRequest<CommentResponse?>;

public class CreateCommentCommandHandler(IAppDbContext db) : IRequestHandler<CreateCommentCommand, CommentResponse?>
{
    public async Task<CommentResponse?> Handle(CreateCommentCommand request, CancellationToken ct)
    {
        var task = await db.Tasks
                           .Include(t => t.Project).ThenInclude(p => p!.Members)
                           .FirstOrDefaultAsync(t => t.Id == request.TaskId && t.ProjectId == request.ProjectId, ct);
        if (task == null || !task.Project!.HasAccess(request.UserId))
            return null;
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            TaskItemId = request.TaskId,
            AuthorId = request.UserId
        };
        db.Comments.Add(comment);
        await db.SaveChangesAsync(ct);
        return new CommentResponse(comment);
    }
}
