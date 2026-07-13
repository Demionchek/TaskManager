using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;

namespace TaskManager.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public class LoginCommandHandler(IAppDbContext db, IJwtService jwtService, IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        if (user == null)
            return Result<AuthResponse>.Fail(ErrorType.Unauthorized, "User not found");
        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Fail(ErrorType.Unauthorized, "Password doesn't match");

        if (user.RefreshToken == null)
        {
            user.RefreshToken = jwtService.GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await db.SaveChangesAsync(ct);
        }
        return Result<AuthResponse>.Success(new AuthResponse(jwtService.GenerateToken(user), user.RefreshToken, user.Username));
    }
}
