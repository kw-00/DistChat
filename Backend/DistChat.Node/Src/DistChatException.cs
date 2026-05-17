namespace DistChat.Node.Exceptions;

public class DistChatException : Exception
{
    public DistChatException() : base() { }
    public DistChatException(string message) : base(message) { }
    public DistChatException(string message, Exception innerException) 
        : base(message, innerException) { }
}