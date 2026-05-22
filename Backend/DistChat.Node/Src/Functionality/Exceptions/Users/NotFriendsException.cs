using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class NotFriendsException(Guid userAId, Guid UserBId)
    : DistChatException(
        $"Users with IDs of \"{userAId}\"" 
        + $" and \"{UserBId}\" are not friends."
    );