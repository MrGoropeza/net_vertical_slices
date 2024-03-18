using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Endpoints;
using WebApi.Endpoints.Abstractions;

namespace WebApi.Application.MessageQueue;

public static class EnqueueEvent
{
    // public record Request : IRequest
    // {
    //     public string Message { get; init; } = string.Empty;
    // }

    // public class Endpoint : IEndpoint
    // {
    //     public void MapEndpoint(IEndpointRouteBuilder app) =>
    //         app.MapPost("messagequeue", EndpointHandler)
    //             .WithName(nameof(EnqueueEvent))
    //             .WithSummary("Enqueue Event")
    //             .WithDescription("Enqueue Event")
    //             .WithTags(Tags.MessageQueue)
    //             .WithOpenApi();

    //     private async Task<IResult> EndpointHandler(ISender sender, [FromBody] Request query)
    //     {
    //         await sender.Send(query);

    //         return Results.Ok();
    //     }
    // }

    // public class Handler(InMemoryMessageQueue queue) : IRequestHandler<Request>
    // {
    //     public async Task Handle(Request request, CancellationToken cancellationToken)
    //     {
    //         await queue.Writer.WriteAsync(new WebsocketEvent(request.Message), cancellationToken);
    //     }
    // }
}
