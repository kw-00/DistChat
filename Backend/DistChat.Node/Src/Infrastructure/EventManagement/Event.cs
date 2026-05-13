namespace DistChat.Node.Infrastructure.EventManagement;

public abstract class Event
{
    public Guid RoomId { get; init; }
    public string Topic => GetType().Name[0..^5];
    public string Address => GetAddress(RoomId, Topic);

    public required EventPayload Payload { get; init; }

    public static string GetAddress(Guid roomId, string topic) => $"{roomId}.{topic}";
}

