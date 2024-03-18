using Microsoft.AspNetCore.Mvc;
using WebApi.Endpoints;
using WebApi.Endpoints.Abstractions;

namespace WebApi.Application.Topics.SSE;

public class SSEEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("sse", Connect).WithTags(Tags.Realtime);
        app.MapPost("sse/subscribe", Subscribe).WithTags(Tags.Realtime);
        app.MapPost("sse/unsubscribe", Unsubscribe).WithTags(Tags.Realtime);
    }

    private static async Task<IResult> Connect(HttpContext context, RealtimeManager connections)
    {
        var socketId = Guid.NewGuid().ToString();
        SSERealtimeSubscription subscription = new(socketId, 1);

        connections.Add(subscription);

        context.Response.Headers.ContentType = "text/event-stream";

        return await subscription.HandleConnection(context);
    }

    public void Subscribe([FromBody] SSETopicSubscription action, RealtimeManager connections)
    {
        foreach (var topic in action.Topics)
        {
            connections.AddTopicToSubscription(action.SocketId, topic);
        }
    }

    public void Unsubscribe([FromBody] SSETopicSubscription action, RealtimeManager connections)
    {
        foreach (var topic in action.Topics)
        {
            connections.RemoveTopicFromSubscription(action.SocketId, topic);
        }
    }
}

public class SSETopicSubscription
{
    public string SocketId { get; set; } = string.Empty;
    public IEnumerable<string> Topics { get; set; } = Enumerable.Empty<string>();
}
