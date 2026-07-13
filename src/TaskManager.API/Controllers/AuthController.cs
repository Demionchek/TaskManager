using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common;
using TaskManager.Application.Features.Auth.Commands;
using LoginRequest = TaskManager.Application.DTOs.LoginRequest;
using RefreshRequest = TaskManager.Application.DTOs.RefreshRequest;
using RegisterRequest = TaskManager.Application.DTOs.RegisterRequest;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : BaseController
{
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new RegisterCommand(request.Username, request.Email, request.Password), ct);
        if (!result.IsSuccess) return ToError(result.Error!.Value, result.Message);
        return Ok(result.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginCommand(request.Email, request.Password), ct);
        if (!result.IsSuccess) return ToError(result.Error!.Value, result.Message);
        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new RefreshCommand(request.RefreshToken), ct);
        if (!result.IsSuccess) return ToError(result.Error!.Value, result.Message);
        return Ok(result.Value);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout(RefreshRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new LogoutCommand(request.RefreshToken), ct);
        if (!result.IsSuccess) return ToError(result.Error!.Value, result.Message);
        return Ok();
    }
}
