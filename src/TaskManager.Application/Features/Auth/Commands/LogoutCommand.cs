using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common;

namespace TaskManager.Application.Features.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;

public class LogoutCommandHandler(IAppDbContext db) : IRequestHandler<LogoutCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.RefreshToken == request.RefreshToken && u.RefreshTokenExpiry > DateTime.UtcNow, ct);
        if (user == null)
            return Result<bool>.Fail(ErrorType.Unauthorized, "No user found");

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
