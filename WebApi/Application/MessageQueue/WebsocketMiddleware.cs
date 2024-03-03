using System.Net.WebSockets;
using System.Text;

namespace WebApi.Application.MessageQueue;

public class WebsocketMiddleware(WebsocketConnections connections) : IMiddleware
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

        connections.Add(socketId, webSocket);

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

            var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            await connections.Send(message);
        }

        await webSocket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            "Closed",
            CancellationToken.None
        );

        connections.Remove(socketId);

        await next(context);
    }
}
