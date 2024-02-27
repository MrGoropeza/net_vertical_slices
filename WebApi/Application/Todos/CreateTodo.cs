using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Endpoints;
using WebApi.Endpoints.Abstractions;
using WebApi.Endpoints.Filters;
using WebApi.Infrastructure.Database;
using WebApi.Models;

namespace WebApi.Application.Todos;

public static class CreateTodo
{
    public record Response(Todo Todo);

    public record Command(string Title, bool IsCompleted) : IRequest<Response>;

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost("todos", EndpointHandler)
                .WithName(nameof(CreateTodo))
                .WithTags(Tags.Todos)
                .WithSummary("Create a new Todo")
                .WithDescription("Create a new Todo")
                .WithOpenApi()
                .AddEndpointFilter<ValidationFilter<Command>>();

        private async Task<IResult> EndpointHandler([FromBody] Command command, ISender sender)
        {
            var response = await sender.Send(command);

            return Results.Ok(response.Todo);
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.IsCompleted).NotNull();
        }
    }

    public class Handler(AppDbContext context) : IRequestHandler<Command, Response>
    {
        private readonly AppDbContext _context = context;

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                IsCompleted = request.IsCompleted
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response(todo);
        }
    }
}
