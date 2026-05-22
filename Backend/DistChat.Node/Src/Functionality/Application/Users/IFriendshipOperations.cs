namespace DistChat.Node.Functionality.Application.Users;

public interface IFriendshipOperations
{

    Task RequestFriendshipAsync(string connectionId, Guid targetUserId);


    Task AcceptFriendshipAsync(string connectionId, Guid requestingUserId);


    Task RejectFriendshipAsync(string connectionId, Guid requestingUserId);


    Task UnfriendAsync(string connectionId, Guid friendId);


    Task HandleConnectedAsync(string connectionId, Guid userId);

    Task HandleDisconnectedAsync(string connectionId, Guid userId);
}