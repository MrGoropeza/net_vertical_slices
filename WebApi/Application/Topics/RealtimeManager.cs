using System.Collections.Concurrent;

namespace WebApi.Application.Topics;

public class RealtimeManager
{
    public ConcurrentDictionary<string, IRealtimeSubscription> Subscriptions { get; } = new();

    public void Add(IRealtimeSubscription subscription)
    {
        Subscriptions.TryAdd(subscription.ConnectionId, subscription);
    }

    public void Remove(string connectionId)
    {
        if (Subscriptions.TryGetValue(connectionId, out var subscription))
        {
            subscription.Close();
            Subscriptions.TryRemove(connectionId, out _);
        }
    }

    public void AddTopicToSubscription(string connectionId, string topic)
    {
        if (Subscriptions.TryGetValue(connectionId, out var subscription))
        {
            subscription.Subscribe(topic);
            subscription.Send($"Topic added: {topic}");
        }
    }

    public void RemoveTopicFromSubscription(string connectionId, string topic)
    {
        if (Subscriptions.TryGetValue(connectionId, out var subscription))
        {
            subscription.Unsubscribe(topic);
            subscription.Send($"Topic removed: {topic}");
        }
    }

    public Task SendToAll(object message)
    {
        var tasks = Subscriptions.Select(s => s.Value.Send(message));

        return Task.WhenAll(tasks);
    }

    public Task SendToTopic(string topic, object message)
    {
        var tasks = Subscriptions
            .Where(s => s.Value.Topics.Contains(topic))
            .Select(s => s.Value.Send(message));

        return Task.WhenAll(tasks);
    }

    public Task SendToUser(long userId, object message)
    {
        var tasks = Subscriptions
            .Where(s => s.Value.UserId == userId)
            .Select(s => s.Value.Send(message));

        return Task.WhenAll(tasks);
    }
}
