using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;

namespace TaskManager.Application.Features.Auth.Commands;

public record RefreshCommand(string RefreshToken) : IRequest<Result<AuthResponse>>;

public class RefreshCommandHandler(IAppDbContext db, IJwtService jwtService)
    : IRequestHandler<RefreshCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshCommand request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.RefreshToken == request.RefreshToken && u.RefreshTokenExpiry > DateTime.UtcNow, ct);
        if (user == null)
            return Result<AuthResponse>.Fail(ErrorType.Unauthorized, "No user found");

        user.RefreshToken = jwtService.GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await db.SaveChangesAsync(ct);
        return Result<AuthResponse>.Success(new AuthResponse(jwtService.GenerateToken(user), user.RefreshToken, user.Username));
    }
}
