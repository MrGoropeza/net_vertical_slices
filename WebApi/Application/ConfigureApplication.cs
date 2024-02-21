using FluentValidation;

namespace WebApi.Application;

public static class ConfigureApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(
            cfg => cfg.RegisterServicesFromAssembly(typeof(ConfigureApplication).Assembly)
        );
        services.AddValidatorsFromAssembly(typeof(ConfigureApplication).Assembly);

        return services;
    }
}
