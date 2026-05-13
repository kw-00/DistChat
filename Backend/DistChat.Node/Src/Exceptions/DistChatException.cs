namespace DistChat.Node.Exceptions;

public class DistChatException : Exception
{
    public DistChatException() : base() { }
    public DistChatException(string message) : base(message) { }
    public class DistChatNotFoundException(string message, Exception innerException) 
        : Exception(message, innerException) { }
}