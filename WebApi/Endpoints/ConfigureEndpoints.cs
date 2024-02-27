using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApi.Endpoints.Abstractions;
using WebApi.Endpoints.Swagger;

namespace WebApi.Endpoints;

public static class ConfigureEndpoints
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        ServiceDescriptor[] serviceDescriptors = assembly.DefinedTypes
            .Where(
                type =>
                    type is { IsAbstract: false, IsInterface: false }
                    && type.IsAssignableTo(typeof(IEndpoint))
            )
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        services.AddSwaggerGen(cfg =>
        {
            cfg.SchemaFilter<AvailableValuesFilter>();
            cfg.SchemaFilter<SortPropertiesFilter>();
        });

        return services;
    }

    public static IApplicationBuilder UseEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null
    )
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
