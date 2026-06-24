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

    public async Task StartConsumptionAsync(string connectionId, Guid listenerGroupId)
    {
        await eventManager.StartConsumptionAsync(
            connectionId, new EventAddress(listenerGroupId, topic)
        );
    }

    public async Task StopConsumptionAsync(string connectionId, Guid listenerGroupId)
    {
        await eventManager.StopConsumptionAsync(
            connectionId, new EventAddress(listenerGroupId, topic)
        );
    }
}