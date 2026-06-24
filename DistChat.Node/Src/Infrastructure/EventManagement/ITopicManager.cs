namespace DistChat.Node.Infrastructure.EventManagement;

public interface ITopicManager
{
    Task StartConsumptionAsync(string connectionId, Guid listenerGroup);
    Task StopConsumptionAsync(string connectionId, Guid listenerGroup);
}