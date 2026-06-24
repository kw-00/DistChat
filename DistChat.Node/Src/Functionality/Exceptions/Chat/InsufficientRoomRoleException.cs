using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Chat;

public class InsufficientRoomRoleException : DistChatException
{
    public InsufficientRoomRoleException(
        Guid userId,
        Guid roomId
    ) : base(
        $"User with ID of {userId} has insufficient permissions within"
        + $" room with ID of {roomId} or is not in the room at all."
    )
    { }
}