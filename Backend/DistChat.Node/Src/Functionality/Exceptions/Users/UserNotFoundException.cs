using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class UserNotFoundException : DistChatException
{
    public UserNotFoundException(Guid id) : base($"User with id of \"{id}\" not found.") { }
    public UserNotFoundException(string message) : base(message) { }

    public UserNotFoundException(string message, Exception innerException) 
        : base(message, innerException) { }
}

