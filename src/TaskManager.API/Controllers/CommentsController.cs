using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using TaskManager.API.Application.DTOs;
using TaskManager.API.Domain.Entities;
using TaskManager.API.Infrastructure.Persistence;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/tasks/{taskId}/comments")]
public class CommentsController : ControllerBase
{
    private AppDbContext db;
    public CommentsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<ActionResult> Get(Guid projectId, Guid taskId)
    {
        TaskItem? taskItem = await GetTaskAsync(projectId,taskId);
        if (taskItem == null)
            return NotFound();
        List<CommentResponse> responses = taskItem.Comments.Select(t => new CommentResponse(t!)).ToList();
        return Ok(responses);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Guid projectId, Guid taskId, CreateCommentRequest request)
    {
        TaskItem? taskItem = await GetTaskAsync(projectId,taskId);
        if (taskItem == null)
            return NotFound();
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            TaskItemId = taskId,
            AuthorId = GetUserId()
        };
        await db.Comments.AddAsync(comment);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Post), new {id = comment.Id}, new CommentResponse(comment));
    }

    [HttpPut("{commentId}")]
    public async Task<ActionResult> Put(Guid projectId, Guid taskId, Guid commentId, CommentUpdateRequest request)
    {
        TaskItem? taskItem = await GetTaskAsync(projectId,taskId);
        if (taskItem == null)
            return NotFound();
        Comment? comment = await db.Comments.FirstOrDefaultAsync(t => t.Id == commentId && t.TaskItemId == taskItem.Id);
        if (comment == null) return NotFound();
        if (comment.AuthorId != GetUserId()) return Forbid();
        comment.Content = request.Content;
        await db.SaveChangesAsync();
        return Ok(new CommentResponse(comment));
    }

    [HttpDelete("{commentId}")]
    public async Task<ActionResult> Delete(Guid projectId, Guid taskId, Guid commentId)
    {
        TaskItem? taskItem = await GetTaskAsync(projectId,taskId);
        if (taskItem == null)
            return NotFound();
        Comment? comment = await db.Comments.FirstOrDefaultAsync(t => t.Id == commentId && t.TaskItemId == taskItem.Id);
        if (comment == null) return NotFound();
        if (comment.AuthorId != GetUserId()) return Forbid();
        db.Comments.Remove(comment);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<TaskItem?> GetTaskAsync(Guid projectId,Guid taskId)
    {
        return await db.Tasks.Include(t => t.Comments)
                       .FirstOrDefaultAsync(t => t.Id == taskId &&  t.ProjectId == projectId);
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
}