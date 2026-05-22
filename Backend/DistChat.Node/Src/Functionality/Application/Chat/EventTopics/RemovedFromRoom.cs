using DistChat.Node.Functionality.Application.Users;
using DistChat.Node.Functionality.Database.Chat;
using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Chat;

public class RemovedFromRoom(IEventManager eventManager)
    : TopicManager<Guid>(eventManager, "addedToRoom")
{ }