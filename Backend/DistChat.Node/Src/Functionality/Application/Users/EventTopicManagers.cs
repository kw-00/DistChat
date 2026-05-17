using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Users;

public class FriendshipRequestedTopicManager
(
    IEventManager eventManager
) : TopicManager<PublicUserDTO>(eventManager, "friendshipRequested")
{
}

public class NewFriendTopicManager(
    IEventManager eventManager
) : TopicManager<PublicUserDTO>(eventManager, "newFriend")
{
}

public class UnfriendedTopicManager(
    IEventManager eventManager
) : TopicManager<Guid>(eventManager, "unfriended")
{
}