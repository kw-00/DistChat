using DistChat.Node.Exceptions;

namespace DistChat.Node.Auth.Services;

public class InvalidRefreshTokenException : DistChatException
{
    public InvalidRefreshTokenException(string message) : base(message) { }
}