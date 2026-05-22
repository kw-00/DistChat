using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class BefriendingSelfException()
    : DistChatException("A user cannot befriend themselves.");