namespace WebApi.Endpoints;

public static class ConfigureEndpoints
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapTodoEndpoints();
    }
}
