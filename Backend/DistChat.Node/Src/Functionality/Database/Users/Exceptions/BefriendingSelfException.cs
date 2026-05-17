using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Database.Users;

public class BefriendingSelfException()
    : DistChatException("A user cannot befriend themselves.");