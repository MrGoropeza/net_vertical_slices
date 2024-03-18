namespace WebApi.Application.Topics;

public static class RealtimeAction
{
    public const string Subscribe = "subscribe";
    public const string Unsubscribe = "unsubscribe";
}

public class RealtimeActionMessage
{
    public required string Action { get; set; }
    public required string[] Topics { get; set; }
}
