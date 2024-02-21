using FluentValidation;
using MediatR;
using WebApi.Infrastructure.Database;
using WebApi.Models;

namespace WebApi.Application.Todos;

public static class CreateTodo
{
    public record Response(Todo Todo);

    public static async Task<IResult> Endpoint(Command command, ISender sender)
    {
        var response = await sender.Send(command);
        return Results.Ok(response.Todo);
    }

    public record Command(string Title, bool? IsCompleted = false) : IRequest<Response>;

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
                IsCompleted = request.IsCompleted ?? false
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync(cancellationToken);

            return new Response(todo);
        }
    }
}
