using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class LoginInUseException(string login)
    : DistChatException($"Login \"{login}\" is already in use.");