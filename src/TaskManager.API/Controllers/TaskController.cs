using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.Queries;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/tasks")]
public class TaskController(IMediator mediator) : BaseController
{

    [HttpGet]
    public async Task<ActionResult> Get(Guid projectId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetTasksQuery(projectId, GetUserId()), ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{taskId}")]
    public async Task<ActionResult> Get(Guid projectId, Guid taskId, CancellationToken ct)
    {
        var taskResponse = await mediator.Send(new GetTaskByIdQuery(projectId, taskId, GetUserId()), ct);
        if (taskResponse == null)
            return NotFound();
        return Ok(taskResponse);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Guid projectId, CreateTaskRequest request, CancellationToken ct)
    {
        var taskResponse = await mediator.Send(new CreateTaskCommand(projectId, GetUserId(), request.Title, request.Description, request.Priority, request.AssigneeId), ct);
        if (taskResponse == null) return NotFound();
        return CreatedAtAction(nameof(Get), new { projectId, taskId = taskResponse.Id }, taskResponse);
    }

    [HttpPut("{taskId}")]
    public async Task<ActionResult> Put(Guid projectId, Guid taskId, UpdateTaskRequest request, CancellationToken ct)
    {
        var taskResponse = await mediator.Send(new UpdateTaskCommand(projectId, GetUserId(), taskId,
            Title: request.Title,
            Description: request.Description,
            Priority: request.Priority,
            AssigneeId: request.AssigneeId,
            Status: request.Status), ct);
        if (taskResponse == null) return NotFound();
        return Ok(taskResponse);
    }

    [HttpDelete("{taskId}")]
    public async Task<ActionResult> Delete(Guid projectId, Guid taskId, CancellationToken ct)
    {
        bool success = await mediator.Send(new DeleteTaskCommand(projectId, taskId, GetUserId()), ct);
        return success ? NoContent() : NotFound();
    }

}