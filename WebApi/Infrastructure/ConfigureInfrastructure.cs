using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Database;

namespace WebApi.Infrastructure;

public static class ConfigureInfrastructure
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<AppDbContext>(
            opt => opt.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
        );

        return services;
    }
}
