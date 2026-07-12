using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common;

public interface IAppDbContext
{
    public DbSet<User> Users {get;}
    public DbSet<Project> Projects {get;}
    public DbSet<TaskItem> Tasks {get;}
    public DbSet<Comment> Comments {get;}
    public DbSet<ProjectMember> ProjectMembers {get;}

    public Task<int> SaveChangesAsync(CancellationToken ct = default);
}