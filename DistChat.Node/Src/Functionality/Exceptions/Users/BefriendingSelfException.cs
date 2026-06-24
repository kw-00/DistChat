using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class BefriendingSelfException(Exception innerException)
    : DistChatException("A user cannot befriend themselves.", innerException);