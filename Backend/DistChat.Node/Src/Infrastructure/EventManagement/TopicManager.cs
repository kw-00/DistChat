using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Users;

public class TopicManager<TData>(IEventManager eventManager, string topic) : ITopicManager
{
    public async Task PublishAsync(Guid listenerGroupId, TData data)
    {
        await eventManager.PublishAsync(
            new Event(new EventAddress(listenerGroupId, topic), data)
        );
    }

    public async Task SubscribeAsync(string connectionId, Guid listenerGroupId)
    {
        await eventManager.SubscribeAsync(
            connectionId, new EventAddress(listenerGroupId, topic)
        );
    }

    public async Task UnsubscribeAsync(string connectionId, Guid listenerGroupId)
    {
        await eventManager.UnsubscribeAsync(
            connectionId, new EventAddress(listenerGroupId, topic)
        );
    }
}