namespace DistChat.Node.Infrastructure.EventManagement;

public interface ITopicManager
{
    Task SubscribeAsync(string connectionId, Guid listenerGroup);
    Task UnsubscribeAsync(string connectionId, Guid listenerGroup);
}