using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services;

public interface IJwtService
{
    public string GenerateToken(User user);
    public string GenerateRefreshToken();
}