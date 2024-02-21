using WebApi.Application.Todos;
using WebApi.Endpoints.Filters;

namespace WebApi.Endpoints;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos").WithTags("Todos");

        group.MapGet("", ListTodos.Endpoint).WithName(nameof(ListTodos)).WithOpenApi();

        group
            .MapPost("", CreateTodo.Endpoint)
            .WithName(nameof(CreateTodo))
            .WithOpenApi()
            .AddEndpointFilter<ValidationFilter<CreateTodo.Command>>();
    }
}
