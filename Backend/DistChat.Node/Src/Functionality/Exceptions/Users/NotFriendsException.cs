using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class NotFriendsException : DistChatException
{
    public NotFriendsException(
        Guid userAId, Guid UserBId
    )
        : base(
            $"Users with IDs of \"{userAId}\"" 
            + $" and \"{UserBId}\" are not friends."
        ) { }

    public NotFriendsException(string message) 
        : base(message) { }
}