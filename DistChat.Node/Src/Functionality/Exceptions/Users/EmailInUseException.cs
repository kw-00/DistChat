using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class EmailInUseException(string email, Exception innerException)
    : DistChatException($"Email \"{email}\" is already in use.", innerException);
