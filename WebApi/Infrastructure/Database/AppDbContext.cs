using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>().Property(t => t.CreatedAt).HasDefaultValueSql("now()");
        modelBuilder.Entity<Todo>().Property(t => t.UpdateAt).HasDefaultValueSql("now()");
        modelBuilder.Entity<Todo>().Property(t => t.IsCompleted).HasDefaultValue(false);
    }
}
