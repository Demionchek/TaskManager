namespace TaskManager.API.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? RefreshToken { get; set; } = string.Empty;
    public DateTime? RefreshTokenExpiry  { get; set; }
    public List<Project> Projects { get; set; } = [];
    public List<TaskItem> AssignedTasks { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
}