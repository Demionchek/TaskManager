using TaskManager.API.Domain.Entities;

namespace TaskManager.API.Application.Services;

public interface IJwtService
{
    public string GenerateToken(User user);
    public string GenerateRefreshToken();
}