using DistChat.Node.Functionality.DTOs.Users;

namespace DistChat.Node.Functionality.Database.Users;

public interface IFriendshipDbService
{
    Task RequestFriendshipAsync(Guid requestingUserId, Guid targetUserId);

    Task AcceptFriendshipAsync(Guid acceptingUserId, Guid requestingUserId);

    Task<IReadOnlyList<PublicUserDTO>> GetFriendsAsync(Guid userId);

    Task<IReadOnlyList<PublicUserDTO>> GetIncomingFriendRequestsAsync(Guid userId);

    Task DeclineFriendRequestAsync(Guid decliningUserId, Guid requestingUserId);

    Task UnfriendAsync(Guid initiatingUserId, Guid friendUserId);
}