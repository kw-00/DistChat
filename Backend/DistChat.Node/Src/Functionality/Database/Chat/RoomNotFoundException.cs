using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Database.Chat;

public class RoomNotFoundException : DistChatException
{
    public RoomNotFoundException(string name) 
        : base($"Room \"{name}\" not found.") { }
}