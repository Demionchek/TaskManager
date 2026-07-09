using TaskManager.Domain.Entities;

namespace TaskManager.Application.DTOs;

public class CommentResponse
{
    public CommentResponse(Comment comment)
    {
        Content = comment.Content;
        Id = comment.Id;
        CreatedAt = comment.CreatedAt;
        TaskItemId = comment.TaskItemId;
        AuthorId = comment.AuthorId;
    }

    public Guid Id { get; set; }
    public Guid TaskItemId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}