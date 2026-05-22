using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Users;

public class FriendshipAccepted(IEventManager eventManager) 
    : TopicManager<FriendRequestDTO>(eventManager, "friendRequestAccepted")
{ }