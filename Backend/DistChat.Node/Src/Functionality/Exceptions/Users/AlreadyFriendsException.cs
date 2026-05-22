using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Exceptions.Users;

public class AlreadyFriendsException(Guid acceptingUserId, Guid requestingUserId)
    : DistChatException(
        $"Users with IDs of \"{requestingUserId}\"" 
        + $" and \"{acceptingUserId}\" are already friends."
    );
