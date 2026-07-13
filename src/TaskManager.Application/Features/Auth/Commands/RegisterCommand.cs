using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Auth.Commands;

public record RegisterCommand(string Username, string Email, string Password) : IRequest<Result<AuthResponse>>;

public class RegisterCommandHandler(IAppDbContext db, IJwtService jwtService, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken ct)
    {
        bool exists = await db.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (exists)
            return Result<AuthResponse>.Fail(ErrorType.BadRequest, "Email already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            PasswordHash = passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow,
            RefreshToken = jwtService.GenerateRefreshToken(),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return Result<AuthResponse>.Success(new AuthResponse(jwtService.GenerateToken(user), user.RefreshToken!, user.Username));
    }
}
