using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApi.Application.Todos;

namespace WebApi.Application;

public static class ConfigureApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ConfigureApplication).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        // ServiceDescriptor[] serviceDescriptors = assembly.DefinedTypes
        //     .Where(
        //         type =>
        //             type
        //                 is { IsAbstract: false, IsInterface: false, IsConstructedGenericType: true }
        //             && type.IsAssignableTo(typeof(IRepository<>))
        //     )
        //     .Select(type => ServiceDescriptor.Scoped(typeof(Repository<>), type))
        //     .ToArray();

        // services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        // services.TryAddEnumerable(serviceDescriptors);

        services.TryAddScoped<TodoRepository>();

        return services;
    }
}
