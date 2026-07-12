using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Projects.Commands;
using TaskManager.Application.Features.Projects.Queries;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectsController(IMediator mediator) : BaseController
{
    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken ct)
    {
        var result = await mediator.Send( new GetProjectsQuery(GetUserId()), ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send( new GetProjectByIdQuery(id, GetUserId()), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateProjectRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateProjectCommand(GetUserId(), request.Name, request.Description), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(Guid id, UpdateProjectRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new UpdateProjectCommand(id, GetUserId(), request.Name, request.Description), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await mediator.Send(new DeleteProjectCommand(id, GetUserId()), ct);
        return deleted ? NoContent() : NotFound();
    }
}