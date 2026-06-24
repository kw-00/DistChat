using DistChat.Node.Functionality.Database.Users;
using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Functionality.Exceptions.Users;
using DistChat.Node.Infrastructure.EventManagement;
using DistChat.Node.Infrastructure.RealtimeHub;

namespace DistChat.Node.Functionality.Application.Users;

public class FriendshipOperations(
    UserConnectionTracker connectionTracker,
    IFriendshipDbService friendshipDbService,
    IUserDbService userDbService,
    FriendshipRequested friendshipRequested,
    FriendshipAccepted friendshipAccepted,
    FriendshipRejected friendshipRejected,
    FriendRemoved friendRemoved
) : IFriendshipOperations
{

    public async Task RequestFriendshipAsync(string connectionId, Guid targetUserId)
    {
        var userId = connectionTracker.GetUserId(connectionId);

        var requestingUser = await userDbService.GetAsync(userId)
            ?? throw new UserNotFoundException(userId);

        var requestingUserDto = new PublicUserDTO(requestingUser);

        await friendshipDbService.RequestFriendshipAsync(userId, targetUserId);
        await friendshipRequested.PublishAsync(userId, requestingUserDto);
    }

    public async Task AcceptFriendshipAsync(
        string connectionId, Guid requestingUserId
    )
    {
        var acceptingUserId = connectionTracker.GetUserId(connectionId);

        var requestingUser = await userDbService.GetAsync(requestingUserId)
            ?? throw new UserNotFoundException(requestingUserId);

        var acceptingUser = await userDbService.GetAsync(acceptingUserId)
            ?? throw new UserNotFoundException(acceptingUserId);

        var acceptingUserDto = new PublicUserDTO(acceptingUser);
        var requestingUserDto = new PublicUserDTO(requestingUser);

        var friendRequestDto = new FriendRequestDTO(
            requestingUserDto, acceptingUserDto
        );

        await friendshipDbService.AcceptFriendshipAsync(
            acceptingUserId, requestingUserId
        );

        await friendshipAccepted.PublishAsync(acceptingUserId, friendRequestDto);
        await friendshipAccepted.PublishAsync(requestingUserId, friendRequestDto);
    }

    public async Task RejectFriendshipAsync(
        string connectionId, Guid requestingUserId
    )
    {
        var rejectingUserId = connectionTracker.GetUserId(connectionId);

        await friendshipDbService.DeclineFriendRequestAsync(
            rejectingUserId, requestingUserId
        );
        await friendshipRejected.PublishAsync(requestingUserId, rejectingUserId);
    }

    public async Task UnfriendAsync(string connectionId, Guid friendId)
    {
        var userId = connectionTracker.GetUserId(connectionId);

        await friendshipDbService.UnfriendAsync(userId, friendId);

        await friendRemoved.PublishAsync(friendId, userId);
        await friendRemoved.PublishAsync(userId, friendId);
    }

    public async Task HandleConnectedAsync(string connectionId, Guid userId)
    {
        ICollection<ITopicManager> topicManagers =
        [
            friendshipRequested,
            friendshipAccepted,
            friendshipRejected,
            friendRemoved
        ];

        foreach (var topicManager in topicManagers)
            await topicManager.StartConsumptionAsync(connectionId, userId);
    }

    public async Task HandleDisconnectedAsync(string connectionId, Guid userId)
    {
        ICollection<ITopicManager> topicManagers =
        [
            friendshipRequested,
            friendshipAccepted,
            friendshipRejected,
            friendRemoved
        ];

        foreach (var topicManager in topicManagers)
            await topicManager.StopConsumptionAsync(connectionId, userId);
    }
}