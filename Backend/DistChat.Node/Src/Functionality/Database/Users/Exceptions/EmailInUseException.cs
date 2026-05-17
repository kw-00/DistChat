using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Database.Users;

public class EmailInUseException(string email)
    : DistChatException($"Email \"{email}\" is already in use.");
