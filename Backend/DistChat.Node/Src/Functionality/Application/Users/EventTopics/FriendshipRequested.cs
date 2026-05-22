using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Users;

public class FriendshipRequested(IEventManager eventManager) 
    : TopicManager<PublicUserDTO>(eventManager, "friendRequestAccepted")
{ }