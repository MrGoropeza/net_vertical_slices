using Microsoft.AspNetCore.Mvc;
using WebApi.Endpoints;
using WebApi.Endpoints.Abstractions;

namespace WebApi.Application.Topics;

public class SendToAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("realtime/sendtoall", EndpointHandler).WithTags(Tags.Realtime);
    }

    private void EndpointHandler(RealtimeManager realtimeManager)
    {
        realtimeManager.SendToAll("Message to all Subscribers!");
    }
}

public class SendToTopic : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("realtime/sendtotopic", EndpointHandler).WithTags(Tags.Realtime);
    }

    private void EndpointHandler(RealtimeManager realtimeManager, [FromQuery] string topic)
    {
        realtimeManager.SendToTopic(topic, $"Message to Topic: {topic}");
    }
}
