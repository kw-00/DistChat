using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class AlreadyFriendsException : Exception
{

    public AlreadyFriendsException(
        Guid acceptingUserId, Guid requestingUserId
    )
    : base(
        $"Users with IDs of \"{requestingUserId}\""
        + $" and \"{acceptingUserId}\" are already friends."
    )
    { }

    public AlreadyFriendsException(
        Guid acceptingUserId, Guid requestingUserId, Exception innerException
    )
    : base(
        $"Users with IDs of \"{requestingUserId}\""
        + $" and \"{acceptingUserId}\" are already friends.",
        innerException
    )
    { }

}
