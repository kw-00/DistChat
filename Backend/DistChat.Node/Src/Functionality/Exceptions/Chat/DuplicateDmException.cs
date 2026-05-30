using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Chat;

public class DuplicateDmException : DistChatException
{
    public DuplicateDmException(
        Guid userAId, Guid userBId, Exception innerException
    )
        : base(
            $"DM for users with IDs of {userAId} and {userBId} already exists.",
            innerException
        )
    { }
}