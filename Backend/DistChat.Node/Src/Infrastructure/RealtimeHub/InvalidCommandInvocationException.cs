using DistChat.Node.Exceptions;

namespace DistChat.Node.Infrastructure.RealtimeHub;

public class InvalidCommandInvocationException : DistChatException
{
    public InvalidCommandInvocationException(string message) : base(message) { }
    public InvalidCommandInvocationException(string message, Exception innerException)
        : base(message, innerException) { }
}