namespace WebApi.Application.Topics;

public interface IRealtimeSubscription
{
    public List<string> Topics { get; set; }
    public string ConnectionId { get; }
    public long UserId { get; }

    public void Subscribe(string topic);
    public void Unsubscribe(string topic);

    public Task Close();
    public Task Send(object message, CancellationToken cancellationToken = default);
}

public static class RealtimeTopics
{
    public const string Topic1 = "Topic1";
    public const string Topic2 = "Topic2";
    public const string Topic3 = "Topic3";
}
