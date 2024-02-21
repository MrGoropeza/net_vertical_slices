using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Database;
using WebApi.Models;

namespace WebApi.Application.Todos;

public static class ListTodos
{
    public record Response(long Count, IEnumerable<Todo> Data);

    public record Query(int? Page = 1, int? PageSize = 10) : IRequest<Response>;

    public static async Task<IResult> Endpoint(ISender sender, [AsParameters] Query query)
    {
        var response = await sender.Send(query);
        return Results.Ok(response);
    }

    public class Handler(AppDbContext context) : IRequestHandler<Query, Response>
    {
        private readonly AppDbContext _context = context;

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var records = _context.Todos.OrderBy(x => x.Id).AsNoTracking();

            var count = await records.LongCountAsync(cancellationToken);

            if (request.Page is not null && request.PageSize is not null)
            {
                var page = request.Page < 1 ? 1 : request.Page.Value;

                records = records.Skip((page - 1) * request.PageSize.Value);

                if (request.PageSize > 0)
                {
                    records = records.Take(request.PageSize.Value);
                }
            }

            var todos = await records.ToListAsync(cancellationToken);

            return new Response(count, todos);
        }
    }
}
