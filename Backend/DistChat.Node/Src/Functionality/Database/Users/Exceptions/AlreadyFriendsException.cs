using DistChat.Node.Exceptions;

namespace DistChat.Node.Functionality.Database.Users;

public class AlreadyFriendsException(Guid requesterUserId, Guid targetUserId)
    : DistChatException(
        $"Users with IDs of \"{requesterUserId}\"" 
        + $" and \"{targetUserId}\" are already friends."
    );
