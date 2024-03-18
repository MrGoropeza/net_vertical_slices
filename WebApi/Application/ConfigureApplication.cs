using FluentValidation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApi.Application.Todos;
using WebApi.Application.Topics;
using WebApi.Application.Topics.Websockets;

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

        // add repositorys
        services.TryAddScoped<TodoRepository>();

        // // add websocket services
        // services.TryAddTransient<WebsocketMiddleware>();
        // services.TryAddSingleton<WebsocketConnections>();

        // // add message queue
        // services.AddSingleton<InMemoryMessageQueue>();
        // services.AddHostedService<WebsocketsBackgroundService>();

        // add Topics Services
        services.TryAddTransient<WebsocketRealtimeMiddleware>();
        services.TryAddSingleton<RealtimeManager>();

        return services;
    }

    public static IApplicationBuilder UseApplication(this WebApplication app)
    {
        app.UseWebSockets();

        // app.UseMiddleware<WebsocketMiddleware>();
        app.UseMiddleware<WebsocketRealtimeMiddleware>();

        return app;
    }
}
