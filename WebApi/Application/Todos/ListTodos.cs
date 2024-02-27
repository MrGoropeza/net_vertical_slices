using FluentValidation;
using MediatR;
using WebApi.Application.DTOs;
using WebApi.Endpoints;
using WebApi.Endpoints.Abstractions;
using WebApi.Endpoints.Filters;
using WebApi.Endpoints.Swagger;
using WebApi.Models;

namespace WebApi.Application.Todos;

public static class ListTodos
{
    public record Query() : ListQuery(), IRequest<ListResponse<Todo>>
    {
        [SortProperties(typeof(Todo))]
        public new string[]? SortBy
        {
            get => base.SortBy;
            set => base.SortBy = value;
        }
    };

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet("todos", EndpointHandler)
                .WithName(nameof(ListTodos))
                .WithSummary("List Todos")
                .WithDescription("List Todos with pagination & multi-sorting")
                .WithTags(Tags.Todos)
                .WithOpenApi()
                .AddEndpointFilter<ValidationFilter<Query>>();

        private async Task<IResult> EndpointHandler(ISender sender, [AsParameters] Query query)
        {
            var response = await sender.Send(query);

            return Results.Ok(response);
        }
    }

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
                        || x.Select(sort => sort.Replace("-", "").Replace("+", ""))
                            .Distinct()
                            .Count() == x.Length
                )
                .WithErrorCode("InvalidSortBy")
                .WithMessage("Cannot sort by duplicate properties.");

            RuleForEach(x => x.SortBy)
                .Must(
                    sort =>
                        typeof(Todo).GetProperty(sort.Replace("-", "").Replace("+", "")) is not null
                )
                .WithErrorCode("InvalidSortBy")
                .WithMessage("Sort '{PropertyValue}' is invalid.");
        }
    }

    public class Handler(TodoRepository todoRepository) : IRequestHandler<Query, ListResponse<Todo>>
    {
        private readonly TodoRepository _todoRepository = todoRepository;

        public async Task<ListResponse<Todo>> Handle(
            Query request,
            CancellationToken cancellationToken
        )
        {
            var todos = await _todoRepository.Matching(request, cancellationToken);

            return todos;
        }
    }
}
