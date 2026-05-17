namespace DistChat.Node.Infrastructure.EventManagement;

public interface IEventManager
{
    Task PublishAsync(Event @event);

    Task SubscribeAsync(string connectionId, EventAddress eventAddress);

    Task UnsubscribeAsync(string connectionId, EventAddress eventAddress);
}
