using DistChat.Node.Infrastructure.RealtimeHub;

namespace DistChat.Node.Functionality.Application.Users;

public class FriendshipRealtimeHandler
{
    public FriendshipRealtimeHandler(
        CommandDispatcher dispatcher,
        UserConnectionTracker connectionTracker,
        FriendshipOperations operations
    )
    {
        var group = new CommandGroup();

        group.RegisterCommand("requestFriendship", async (invocation) =>
        {
            bool targetUserIdIsValid = Guid.TryParse(
                invocation.Args[0].ToString() ?? "", out var targetUserId
            );
            if (!targetUserIdIsValid)
                throw new InvalidCommandInvocationException(
                    "Target user ID is not a valid Guid."
                );

            await operations.RequestFriendshipAsync(
                invocation.ConnectionId,
                targetUserId
            );
        });

        group.RegisterCommand("acceptFriendship", async (invocation) =>
        {
            bool requestingUserIdIsValid = Guid.TryParse(
                invocation.Args[0].ToString() ?? "", out var requestingUserId
            );
            if (!requestingUserIdIsValid)
                throw new InvalidCommandInvocationException(
                    "Requesting user ID is not a valid Guid."
                );

            await operations.AcceptFriendshipAsync(
                invocation.ConnectionId,
                requestingUserId
            );
        });

        group.RegisterCommand("rejectFriendship", async (invocation) =>
        {
            var requesterIdIsValid = Guid.TryParse(
                invocation.Args[0].ToString() ?? "", out var requestingUserId
            );
            if (!requesterIdIsValid)
                throw new InvalidCommandInvocationException(
                    "Requesting user ID is not a valid Guid."
                );

            await operations.RejectFriendshipAsync(
                invocation.ConnectionId,
                requestingUserId
            );
        });

        group.RegisterCommand("unfriend", async (invocation) =>
        {
            var friendIdIsValid = Guid.TryParse(
                invocation.Args[0].ToString() ?? "", out var friendId
            );
            if (!friendIdIsValid)
                throw new InvalidCommandInvocationException(
                    "Friend user ID is not a valid Guid."
                );

            await operations.UnfriendAsync(
                invocation.ConnectionId,
                friendId
            );
        });

        connectionTracker.UserConnected += async (conn) =>
        {
            await operations.HandleConnectedAsync(
                conn.ConnectionId, conn.UserId
            );
        };

        connectionTracker.UserDisconnected += async (conn) =>
        {
            await operations
                .HandleDisconnectedAsync(
                    conn.ConnectionId, conn.UserId
                );
        };

        dispatcher.RegisterGroup("friendship", group);
    }
}