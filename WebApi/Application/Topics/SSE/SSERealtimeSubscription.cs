using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace WebApi.Application.Topics.SSE;

public class SSERealtimeSubscription(string connectionId, long userId) : IRealtimeSubscription
{
    public List<string> Topics { get; set; } = new();
    public string ConnectionId { get; } = connectionId;
    public long UserId { get; } = userId;

    private readonly Channel<object> _messagesChannel = Channel.CreateBounded<object>(
        new BoundedChannelOptions(5)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = true
        }
    );

    private ChannelReader<object> Reader => _messagesChannel.Reader;
    private ChannelWriter<object> Writer => _messagesChannel.Writer;

    public async Task<IResult> HandleConnection(HttpContext context)
    {
        var firstMessage = new StringBuilder()
            .AppendLine("event: connection")
            .AppendLine($"data: {ConnectionId}\n")
            .ToString();

        await context.Response.WriteAsync(firstMessage);

        await foreach (var message in Reader.ReadAllAsync(context.RequestAborted))
        {
            JsonSerializerOptions options =
                new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var serializedMessage = JsonSerializer.Serialize(message, options: options);

            var sseMessage = new StringBuilder()
                .AppendLine($"data: {serializedMessage}\n")
                .ToString();

            await context.Response.WriteAsync(
                sseMessage,
                cancellationToken: context.RequestAborted
            );
        }

        await context.Response.WriteAsync(
            new StringBuilder().AppendLine($"data: connection closed\n").ToString()
        );

        return Results.NoContent();
    }

    public async Task Close()
    {
        await Writer.WriteAsync("Connection closed");
        Writer.Complete();
    }

    public async Task Send(object message, CancellationToken cancellationToken = default)
    {
        JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var serializedMessage = JsonSerializer.Serialize(message, options: options);

        await Writer.WriteAsync(serializedMessage, cancellationToken);
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
