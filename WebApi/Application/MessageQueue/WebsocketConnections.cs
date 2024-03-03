using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace WebApi.Application.MessageQueue;

public class WebsocketConnections
{
    public ConcurrentDictionary<string, WebSocket> Connections { get; } = new();

    public void Add(string id, WebSocket webSocket)
    {
        Connections.TryAdd(id, webSocket);
    }

    public void Remove(string id)
    {
        Connections.TryRemove(id, out _);
    }

    public async Task Send(string message)
    {
        var tasks = Connections.Select(
            x =>
                Task.Delay(3000)
                    .ContinueWith(
                        _ =>
                            x.Value.SendAsync(
                                new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None
                            )
                    )
        );

        await Task.WhenAll(tasks);
    }
}
