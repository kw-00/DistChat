using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class LoginInUseException(string login, Exception innerException)
    : DistChatException($"Login \"{login}\" is already in use.", innerException);