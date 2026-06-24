using DistChat.Node.Functionality.Application.Users;
using DistChat.Node.Functionality.DTOs.Chat;
using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Chat;

public class UserJoined(IEventManager eventManager)
    : TopicManager<UserJoinedtDTO>(eventManager, "userJoined")
{ }