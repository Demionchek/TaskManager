using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using TaskManager.Domain.Entities;

namespace TaskManager.API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected Guid GetUserId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    protected bool HasProjectAccess(Project? project, Guid userId)
    {
        if (project == null) return false;
        return project.OwnerId == userId || project.Members.Any(m => m.UserId == userId);
    }
}