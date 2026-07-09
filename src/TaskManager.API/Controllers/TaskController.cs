using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/tasks")]
public class TaskController : BaseController
{
    private AppDbContext db;
    public TaskController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<ActionResult> Get(Guid projectId)
    {
        List<TaskItem>? taskItems = await GetProjectTasksAsync(projectId);
        if (taskItems == null)
            return NotFound();
        List<TaskResponse> responses = taskItems.Select(t => new TaskResponse(t)).ToList();
        return Ok(responses);
    }

    [HttpGet("{taskId}")]
    public async Task<ActionResult> Get(Guid projectId, Guid taskId)
    {
        TaskItem? taskItem = await GetTaskItemAsync(projectId, taskId);
        if (taskItem == null)
            return NotFound();
        return Ok(new TaskResponse(taskItem));
    }

    [HttpPost]
    public async Task<ActionResult> Post(Guid projectId, CreateTaskRequest request)
    {
        Project? project = await GetProjectAsync(projectId);
        if (project == null)
            return NotFound();
        TaskItem taskItem = new TaskItem();
        await MapTaskItemCreate(request, taskItem, project);
        await db.Tasks.AddAsync(taskItem);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Post),new {id = taskItem.Id},new TaskResponse(taskItem));
    }

    [HttpPut("{taskId}")]
    public async Task<ActionResult> Put(Guid projectId, Guid taskId, UpdateTaskRequest request)
    {
        TaskItem? taskItem = await GetTaskItemAsync(projectId, taskId);
        if (taskItem == null)
            return NotFound();
        if (!HasProjectAccess(taskItem.Project!, GetUserId())) return Forbid();
        taskItem.AssigneeId = request.AssigneeId;
        taskItem.Assignee = await GetUserByGuid(request.AssigneeId);
        taskItem.Status = request.Status;
        taskItem.Priority = request.Priority;
        taskItem.Description = request.Description;
        taskItem.Title = request.Title;
        await db.SaveChangesAsync();
        return Ok(new TaskResponse(taskItem));
    }

    [HttpDelete("{taskId}")]
    public async Task<ActionResult> Delete(Guid projectId, Guid taskId)
    {
        TaskItem? taskItem = await GetTaskItemAsync(projectId, taskId);
        if (taskItem == null)
            return NotFound();
        if (!HasProjectAccess(taskItem.Project!, GetUserId())) return Forbid();
        taskItem.Project!.Tasks.Remove(taskItem);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task MapTaskItemCreate(CreateTaskRequest request, TaskItem taskItem, Project project)
    {
        taskItem.Id = Guid.NewGuid();
        taskItem.Title = request.Title;
        taskItem.Description = request.Description;
        taskItem.Priority = request.Priority;
        taskItem.AssigneeId = request.AssigneeId;
        taskItem.Status = StatusEnum.Todo;
        taskItem.CreatedAt = DateTime.UtcNow;
        taskItem.Assignee = await GetUserByGuid(taskItem.AssigneeId);
        taskItem.Project = project;
        taskItem.ProjectId = project.Id;
    }

    private async Task<Project?> GetProjectAsync(Guid projectId)
    {
        Project? project = await db.Projects.Include(p=> p.Members)
                                   .FirstOrDefaultAsync(p => p.Id == projectId);
        var userId = GetUserId();
        if (project == null || !HasProjectAccess(project, userId)) return null;
        return project;
    }

    private async Task<List<TaskItem>?> GetProjectTasksAsync(Guid projectId)
    {
        Project? project = await db.Projects.Include(p => p.Tasks)
                                   .ThenInclude(t => t.Project)
                                   .Include(p => p.Members)
                                   .FirstOrDefaultAsync(p => p.Id == projectId);
        var userId = GetUserId();
        if (project == null || !HasProjectAccess(project, userId) ) return null;
        return project.Tasks;
    }


    private async Task<TaskItem?> GetTaskItemAsync(Guid projectId, Guid taskId)
    {
        var taskItems = await GetProjectTasksAsync(projectId);
        if (taskItems == null) return null;
        TaskItem? taskItem =  taskItems.FirstOrDefault(t => t.Id == taskId);
        if (taskItem == null) return null;
        return taskItem;
    }

    private async Task<User?> GetUserByGuid(Guid? id)
    {
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user;
    }
}