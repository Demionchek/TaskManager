using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Comments.Commands;
using TaskManager.Application.Features.Comments.Queries;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/tasks/{taskId}/comments")]
public class CommentsController(IMediator mediator) : BaseController
{
    [HttpGet]
    public async Task<ActionResult> Get(Guid projectId, Guid taskId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCommentsQuery(projectId, taskId, GetUserId()), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Guid projectId, Guid taskId, CreateCommentRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateCommentCommand(projectId, taskId, GetUserId(), request.Content), ct);
        if (result == null) return NotFound();
        return CreatedAtAction(nameof(Get), new { projectId, taskId }, result);
    }

    [HttpPut("{commentId}")]
    public async Task<ActionResult> Put(Guid projectId, Guid taskId, Guid commentId, CommentUpdateRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateCommentCommand(projectId, taskId, commentId, GetUserId(), request.Content), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{commentId}")]
    public async Task<ActionResult> Delete(Guid projectId, Guid taskId, Guid commentId, CancellationToken ct)
    {
        var deleted = await mediator.Send(new DeleteCommentCommand(projectId, taskId, commentId, GetUserId()), ct);
        return deleted ? NoContent() : NotFound();
    }
}
