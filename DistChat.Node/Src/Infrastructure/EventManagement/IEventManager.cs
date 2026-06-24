namespace DistChat.Node.Infrastructure.EventManagement;

public interface IEventManager
{
    Task PublishAsync(Event @event);

    Task StartConsumptionAsync(string connectionId, EventAddress eventAddress);

    Task StopConsumptionAsync(string connectionId, EventAddress eventAddress);
}
