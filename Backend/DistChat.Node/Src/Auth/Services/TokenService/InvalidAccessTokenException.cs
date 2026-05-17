using DistChat.Node.Exceptions;

namespace DistChat.Node.Auth.Services;

public class InvalidAccessTokenException : DistChatException
{
    public InvalidAccessTokenException(string message) : base(message) { }
    public InvalidAccessTokenException(string message, Exception innerException)
        : base(message, innerException) { }
}