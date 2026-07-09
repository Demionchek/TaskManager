using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.DTOs;
using LoginRequest = TaskManager.Application.DTOs.LoginRequest;
using RefreshRequest = TaskManager.Application.DTOs.RefreshRequest;
using RegisterRequest = TaskManager.Application.DTOs.RegisterRequest;

namespace TaskManager.API.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private AppDbContext db;
    private IJwtService jwtService;
    public AuthController(AppDbContext db, IJwtService jwtService)
    {
        this.db = db;
        this.jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        bool exists = await db.Users.AnyAsync(u => u.Email == request.Email);
        if (exists) return BadRequest("Email already exists");

        User user = new User()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            RefreshToken = jwtService.GenerateRefreshToken(),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        AuthResponse response = new AuthResponse(jwtService.GenerateToken(user),user.RefreshToken, user.Username);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return Unauthorized("User not found");

        bool verified = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!verified) return Unauthorized("Password doesn't match");
        if (user.RefreshToken == null)
        {
            user.RefreshToken = jwtService.GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await db.SaveChangesAsync();
        }
        AuthResponse response = new AuthResponse(jwtService.GenerateToken(user), user.RefreshToken, user.Username);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(RefreshRequest request)
    {
        User? user = await db.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
        if (user == null) return Unauthorized("No user found");

        user.RefreshToken = jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await db.SaveChangesAsync();
        AuthResponse response = new AuthResponse(jwtService.GenerateToken(user), user.RefreshToken, user.Username);
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout(RefreshRequest request)
    {
        User? user = await db.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
        if (user == null) return Unauthorized("No user found");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await db.SaveChangesAsync();
        return Ok();
    }
}