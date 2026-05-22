using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class RedundantFriendRequestException(Guid requestingUserId, Guid targetUserId)
    : DistChatException(
        $"Cannot create friend request from user with ID of \"{requestingUserId}\"" 
        + $" to user wiTH ID of \"{targetUserId}\"."
        + " Either an equivalent friend request already exists," 
        + "or an opposite request exists." 
        + "Duplicate or mutual friend requests are not alloeed."
    );