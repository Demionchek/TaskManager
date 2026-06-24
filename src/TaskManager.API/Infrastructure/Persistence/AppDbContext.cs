using Microsoft.EntityFrameworkCore;
using TaskManager.API.Domain.Entities;

namespace TaskManager.API.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
}