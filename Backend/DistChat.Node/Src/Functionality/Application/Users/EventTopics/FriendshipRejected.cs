using DistChat.Node.Functionality.Application.Users;
using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Infrastructure.EventManagement;

public class FriendshipRejected(IEventManager eventManager) 
    : TopicManager<Guid>(eventManager, "friendRequestRejected")
{ }