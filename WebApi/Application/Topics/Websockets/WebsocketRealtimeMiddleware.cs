using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebApi.Application.Topics.Websockets;

public class WebsocketRealtimeMiddleware(RealtimeManager connections) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await next(context);
            return;
        }

        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var socketId = Guid.NewGuid().ToString();

        WebsocketRealtimeSubscription subscription = new(webSocket, socketId, 1);

        connections.Add(subscription);

        while (webSocket.State == WebSocketState.Open)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            if (receiveResult.MessageType != WebSocketMessageType.Text)
            {
                continue;
            }

            var stringMessage = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

            JsonSerializerOptions options =
                new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var action = JsonSerializer.Deserialize<RealtimeActionMessage>(stringMessage, options);

            if (action is not null and { Action: RealtimeAction.Subscribe })
            {
                foreach (var topic in action.Topics)
                {
                    connections.AddTopicToSubscription(socketId, topic);
                }
            }

            if (action is not null and { Action: RealtimeAction.Unsubscribe })
            {
                foreach (var topic in action.Topics)
                {
                    connections.RemoveTopicFromSubscription(socketId, topic);
                }
            }
        }

        connections.Remove(socketId);

        await next(context);
    }
}
