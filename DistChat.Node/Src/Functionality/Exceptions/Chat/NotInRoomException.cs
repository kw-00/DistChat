using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Chat;

public class NotInRoomException(Guid userId, Guid roomId)
    : DistChatException(
        $"User with ID of \"{userId}\" is not in room with ID of \"{roomId}\"."
    );