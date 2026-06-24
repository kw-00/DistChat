namespace DistChat.Node.Infrastructure.EventManagement;

public class EventAddress
{
    public Guid ListenerGroup { get; }
    public string Topic { get; }


    public EventAddress(Guid listenerGroup, string topic)
    {
        ListenerGroup = listenerGroup;
        Topic = topic;
    }

    public string Serialize() => $"{ListenerGroup}:{Topic}";

    public static EventAddress Deserialize(string serialized)
    {
        var split = serialized.Split(':', 2);
        return new EventAddress(Guid.Parse(split[0]), split[1]);
    }
}