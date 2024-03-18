using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebApi.Application.Topics.Websockets;

public class WebsocketRealtimeSubscription(WebSocket webSocket, string connectionId, long userId)
    : IRealtimeSubscription
{
    public List<string> Topics { get; set; } = new();
    public string ConnectionId { get; } = connectionId;
    public long UserId { get; } = userId;

    public async Task Close()
    {
        await webSocket.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes("Connection closed")),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );

        await webSocket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            string.Empty,
            CancellationToken.None
        );
    }

    public Task Send(object message, CancellationToken cancellationToken = default)
    {
        if (webSocket.State != WebSocketState.Open)
        {
            return Task.CompletedTask;
        }

        JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var serializedMessage = JsonSerializer.Serialize(message, options: options);

        return webSocket.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(serializedMessage)),
            WebSocketMessageType.Text,
            true,
            cancellationToken
        );
    }

    public void Subscribe(string topic)
    {
        if (Topics.Contains(topic))
            return;

        Topics.Add(topic);
    }

    public void Unsubscribe(string topic)
    {
        Topics.Remove(topic);
    }
}
