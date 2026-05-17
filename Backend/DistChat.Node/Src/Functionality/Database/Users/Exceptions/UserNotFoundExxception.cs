using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Database.Users;

public class UserNotFoundException(Guid id) 
    : DistChatException($"User with id of \"{id}\" not found.");