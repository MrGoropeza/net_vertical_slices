using WebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }
}
