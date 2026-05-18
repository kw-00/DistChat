using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Database.Users;

public class UserNotFoundException : DistChatException
{
    public UserNotFoundException(Guid id) : base($"User with id of \"{id}\" not found.") { }
    public UserNotFoundException(string message) : base(message) { }
}

