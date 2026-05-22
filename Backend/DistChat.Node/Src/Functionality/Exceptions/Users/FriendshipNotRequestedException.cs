using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class FriendshipNotRequestedException(
    Guid supposedRequesterUserId, 
    Guid supposedTargetUserId
) 
    : DistChatException(
        $"User with ID of \"{supposedRequesterUserId}\" did not request friendship"
        +  $" with user with ID of \"{supposedTargetUserId}\"."
    );