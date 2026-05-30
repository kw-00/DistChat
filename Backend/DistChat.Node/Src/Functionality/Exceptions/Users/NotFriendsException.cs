using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class NotFriendsException : DistChatException
{
    public NotFriendsException(
        Guid userAId, Guid userBId
    )
        : base(
            GetMessage(userAId, userBId)
        )
    { }

    public NotFriendsException(
        Guid userAId, Guid userBId, Exception innerException
    )
        : base(
            GetMessage(userAId, userBId),
            innerException
        )
    { }


    public NotFriendsException(string message)
        : base(message) { }

    private static string GetMessage(Guid userAId, Guid userBId)
    {
        return
            $"Users with IDs of \"{userAId}\""
            + $" and \"{userBId}\" are not friends.";
    }
}