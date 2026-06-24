using DistChat.Node.Functionality.Application.Users;
using DistChat.Node.Functionality.DTOs.Chat;
using DistChat.Node.Infrastructure.EventManagement;

namespace DistChat.Node.Functionality.Application.Chat;

public class UserLeft(IEventManager eventManager)
    : TopicManager<UserLeftDTO>(eventManager, "userLeft")
{ }
