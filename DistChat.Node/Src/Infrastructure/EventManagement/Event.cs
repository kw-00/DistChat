namespace DistChat.Node.Infrastructure.EventManagement;

public class Event
{
    public EventAddress Address { get; }

    public object? Data { get; }

    public Event(EventAddress address, object? data)
    {
        Address = address;
        Data = data;
    }
}

