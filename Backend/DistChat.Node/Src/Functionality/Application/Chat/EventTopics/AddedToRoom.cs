using DistChat.Node.Functionality.Application.Users;
using DistChat.Node.Functionality.Database.Chat;
using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Chat;

public class AddedToRoom(IEventManager eventManager)
    : TopicManager<Room>(eventManager, "addedToRoom")
{ }