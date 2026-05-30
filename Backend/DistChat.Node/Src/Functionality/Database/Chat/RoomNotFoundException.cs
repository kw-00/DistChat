using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Database.Chat;

public class RoomNotFoundException : DistChatException
{
    public RoomNotFoundException(Guid id, Exception innerException)
        : base($"Room \"{id}\" not found.", innerException) { }
}