using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using TaskManager.API.Application.DTOs;
using TaskManager.API.Application.Services;
using TaskManager.API.Domain.Entities;
using TaskManager.API.Infrastructure.Persistence;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private AppDbContext db;
    public ProjectsController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        var projects = await GetProjectsByUserGuid();
        List<ProjectResponse> responses = projects!.Select(p => new ProjectResponse(p))
                                                  .ToList();
        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid id)
    {
        var project = await db.Projects
                              .Where(p => p.Id == id && p.OwnerId == GetUserId())
                              .FirstOrDefaultAsync();
        if (project == null) return NotFound();
        ProjectResponse? response = new ProjectResponse(project);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> Post(CreateProjectRequest request)
    {
        var userId = GetUserId();
        User user = (await GetUserByGuid())!;
        Project project = new Project {
            Id = Guid.NewGuid(),
            OwnerId = userId,
            Owner = user,
            CreatedAt = DateTime.UtcNow,
            Name = request.Name,
            Description = request.Description
        };
        user.Projects.Add(project);
        db.Add(project);
        await db.SaveChangesAsync();
        ProjectResponse response = new ProjectResponse(project);

        return CreatedAtAction(nameof(Get), new { id = project.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(Guid id, UpdateProjectRequest request)
    {
        var project = await db.Projects.Where(p => p.Id == id).SingleOrDefaultAsync();
        if (project == null) return NotFound();
        if (project.OwnerId != GetUserId()) return Forbid();
        project.Name = request.Name;
        project.Description = request.Description;
        ProjectResponse response = new ProjectResponse(project);
        await db.SaveChangesAsync();
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var project = await db.Projects.Where(p => p.Id == id).SingleOrDefaultAsync();
        if (project == null) return NotFound();
        if (project.OwnerId != GetUserId()) return Forbid();
        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<List<Project>?> GetProjectsByUserGuid()
    {
        var userId = GetUserId();
        List<Project>? project = await db.Projects.Where(p => p.OwnerId == userId).ToListAsync();
        return project;
    }

    private async Task<User?> GetUserByGuid()
    {
        var userId = GetUserId();
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        return user;
    }

    private Guid GetUserId()
    {
        var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
        return userId;
    }
}