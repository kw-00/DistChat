namespace DistChat.Node.Infrastructure.EventManagement;

public interface IEventManager
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event;

    Task SubscribeAsync(string connectionId, Guid roomId, string topic);

    Task UnsubscribeAsync(string connectionId, Guid roomId, string topic);
}
