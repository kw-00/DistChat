using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Users;

public class FriendRemoved(IEventManager eventManager) 
    : TopicManager<Guid>(eventManager, "friendRemoved")
{ }