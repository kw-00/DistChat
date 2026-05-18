using DistChat.Node.Functionality.Database.Users;
using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Infrastructure.EventManagement;
using DistChat.Node.Infrastructure.RealtimeHub;

namespace DistChat.Node.Functionality.Application.Users;

public class FriendshipRealtimeHandler
{
    public FriendshipRealtimeHandler(
        CommandDispatcher dispatcher,
        ConnectionTracker connectionTracker,
        IFriendshipDbService friendshipDbService,
        IUserDbService userDbService,
        FriendshipRequestedTopicManager friendshipRequestedTopicManger,
        NewFriendTopicManager newFriendTopicManager,
        UnfriendedTopicManager unfriendedTopicManager

    ) 
    {
        var group = new CommandGroup();
        group.RegisterCommand("requestFriendship", async (invocation) =>
        {
            throw new NotImplementedException();
        });

        group.RegisterCommand("acceptFriendship", async (invocation) =>
        {
            throw new NotImplementedException();
        });

        group.RegisterCommand("declineFriendship", async (invocation) =>
        {
            throw new NotImplementedException();
        });

        group.RegisterCommand("unfriend", async (invocation) =>
        {
            throw new NotImplementedException();
        });

        ICollection<ITopicManager> topicManagers = [
            friendshipRequestedTopicManger,
            newFriendTopicManager,
            unfriendedTopicManager
        ];
        connectionTracker.UserConnected += async (conn) =>
        {
            foreach (var topicManager in topicManagers)
                await topicManager.StartConsumptionAsync(conn.ConnectionId, conn.UserId);
        };

        connectionTracker.UserDisconnected += async (conn) =>
        {
            foreach (var topicManager in topicManagers)
                await topicManager.StopConsumptionAsync(conn.ConnectionId, conn.UserId);
        };

        dispatcher.RegisterGroup("friendship", group);
    }
    
}