using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using TaskManager.Application.Common;

namespace TaskManager.API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected Guid GetUserId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    protected ActionResult ToError(ErrorType error, string? message) => error switch
    {
        ErrorType.NotFound => NotFound(message),
        ErrorType.Forbidden => Forbid(),
        ErrorType.Unauthorized => Unauthorized(message),
        _ => BadRequest(message)
    };
}
