using DistChat.Node.Exceptions;

namespace DistChat.Node.Domain.Users;

public class EmailInUseException(string email)
    : DistChatException($"Email \"{email}\" is already in use.");
