using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId}/members")]
public class ProjectMembersController : ControllerBase
{
    private AppDbContext db;

    public ProjectMembersController(AppDbContext db)
    {
        this.db = db;
    }

    [HttpGet]
    public async Task<ActionResult> Get(Guid projectId)
    {
        Project? project = await GetProjectWithMembers(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != GetUserId())  return Forbid();
        List<ProjectMemberResponse> responses = project.Members.Select(m=> new ProjectMemberResponse(m))
                                                       .ToList();
        return Ok(responses);
    }

    [HttpPost]
    public async Task<ActionResult> Post(Guid projectId, AddMemberRequest request)
    {
        Project? project = await GetProjectWithMembers(projectId);
        if (project == null) return NotFound();
        if (project.OwnerId != GetUserId())  return Forbid();
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user == null) return BadRequest("No user found");
        if (project.Members.Any(x => x.UserId == request.UserId)) return BadRequest("User already a member");
        ProjectMember member = new ProjectMember
        {
            UserId = request.UserId,
            ProjectId = projectId,
            Role = request.Role
        };
        await db.ProjectMembers.AddAsync(member);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Post), new { projectId = project.Id, memberId = member.UserId }, new  ProjectMemberResponse(member));
    }

    [HttpDelete("{userId}")]
    public async Task<ActionResult> Delete(Guid projectId, Guid userId)
    {
        if (userId == GetUserId()) return BadRequest("Cannot remove yourself");
        Project? project = await GetProjectWithMembers(projectId);
        if (project == null) return NotFound("Project not found");
        if (project.OwnerId != GetUserId())  return Forbid();
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return BadRequest("No user found");
        ProjectMember? member = project.Members.FirstOrDefault(x => x.UserId == userId);
        if (member == null) return BadRequest("User is not a member");
        db.ProjectMembers.Remove(member);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<Project?> GetProjectWithMembers(Guid projectId)
    {
        Project? project = await db.Projects.Include(p => p.Members)
                                   .FirstOrDefaultAsync(p=> p.Id == projectId);
        return project;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
}