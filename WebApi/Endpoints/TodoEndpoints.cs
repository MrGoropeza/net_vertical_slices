using WebApi.Application.Todos;
using WebApi.Endpoints.Abstractions;
using WebApi.Endpoints.Filters;

namespace WebApi.Endpoints;

public class TodoEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/todos").WithTags("Todos").WithOpenApi();

        group
            .MapGet("", ListTodos.Endpoint)
            .WithName(nameof(ListTodos))
            .AddEndpointFilter<ValidationFilter<ListTodos.Query>>();

        group
            .MapPost("", CreateTodo.Endpoint)
            .WithName(nameof(CreateTodo))
            .AddEndpointFilter<ValidationFilter<CreateTodo.Command>>();
    }
}
