using System.Linq.Dynamic.Core;
using FluentValidation;
using MediatR;
using WebApi.Application.DTOs;
using WebApi.Endpoints.Swagger;
using WebApi.Models;

namespace WebApi.Application.Todos;

public static class ListTodos
{
    public record Response : ListResponse<Todo>
    {
        public Response(ListResponse<Todo> original)
            : base(original) { }
    }

    public record Query() : ListQuery(), IRequest<Response>
    {
        [SortProperties(typeof(Todo))]
        public new string[]? SortBy
        {
            get => base.SortBy;
            set => base.SortBy = value;
        }
    };

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithErrorCode("InvalidPage")
                .WithMessage("Page cannot be less than 1.");
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(0)
                .WithErrorCode("InvalidPageSize")
                .WithMessage("PageSize cannot be less than 0.");
            RuleFor(x => x.SortBy)
                .Must(
                    x =>
                        x is null
                        || x.All(
                            property =>
                                typeof(Todo).GetProperty(property.Replace("-", "").Replace("+", ""))
                                    is not null
                        )
                )
                .WithErrorCode("InvalidSortsBy")
                .WithMessage("Sorts are invalid.");
        }
    }

    public static async Task<IResult> Endpoint(ISender sender, [AsParameters] Query query)
    {
        var response = await sender.Send(query);

        return Results.Ok(response);
    }

    public class Handler(TodoRepository todoRepository) : IRequestHandler<Query, Response>
    {
        private readonly TodoRepository _todoRepository = todoRepository;

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var todos = await _todoRepository.Matching(request, cancellationToken);

            return new Response(todos);
        }
    }
}
