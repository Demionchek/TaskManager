using Microsoft.AspNetCore.Mvc;
using TaskManager.API.Application.DTOs;
using TaskManager.API.Application.Services;
using TaskManager.API.Domain.Entities;
using TaskManager.API.Infrastructure.Persistence;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

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
        bool exists = await db.Users.AnyAsync(x => x.Email == request.Email);
        if (exists) return BadRequest("Email already exists");

        User user = new User()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        AuthResponse response = new AuthResponse(jwtService.GenerateToken(user), user.Username);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginRequest request)
    {
        User? user = await db.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user == null) return Unauthorized("User not found");

        bool verified = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!verified) return Unauthorized("Password doesn't match");
        AuthResponse response = new AuthResponse(jwtService.GenerateToken(user), user.Username);
        return Ok(response);
    }
}