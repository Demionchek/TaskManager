using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Members.Commands;
using TaskManager.Application.Features.Members.Queries;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/members")]
public class ProjectMembersController(IMediator mediator) : BaseController
{
    [HttpGet]
    public async Task<ActionResult> Get(Guid projectId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetMembersQuery(projectId, GetUserId()), ct);
        if (!result.IsSuccess) return ToError(result.Error!.Value, result.Message);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Guid projectId, AddMemberRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new AddMemberCommand(projectId, GetUserId(), request.UserId, request.Role), ct);
        if (!result.IsSuccess) return ToError(result.Error!.Value, result.Message);
        return CreatedAtAction(nameof(Get), new { projectId }, result.Value);
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> Delete(Guid projectId, Guid userId, CancellationToken ct)
    {
        var result = await mediator.Send(new RemoveMemberCommand(projectId, GetUserId(), userId), ct);
        if (!result.IsSuccess) return ToError(result.Error!.Value, result.Message);
        return NoContent();
    }
}
