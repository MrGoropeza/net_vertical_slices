using System.Threading.Channels;
using MediatR;

namespace WebApi.Application.MessageQueue;

public interface IIntegrationEvent : INotification
{
    string Message { get; init; }
}

public record WebsocketEvent(string Message) : IIntegrationEvent;

public sealed class InMemoryMessageQueue
{
    private readonly Channel<IIntegrationEvent> _channel =
        Channel.CreateUnbounded<IIntegrationEvent>();

    public ChannelReader<IIntegrationEvent> Reader => _channel.Reader;

    public ChannelWriter<IIntegrationEvent> Writer => _channel.Writer;
}

internal sealed class WebsocketsBackgroundService(
    InMemoryMessageQueue queue,
    WebsocketConnections connections,
    ILogger<WebsocketsBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (
            IIntegrationEvent integrationEvent in queue.Reader.ReadAllAsync(stoppingToken)
        )
        {
            try
            {
                await connections.Send(integrationEvent.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Something went wrong! {IntegrationEventId}",
                    integrationEvent.Message
                );
            }
        }
    }
}
