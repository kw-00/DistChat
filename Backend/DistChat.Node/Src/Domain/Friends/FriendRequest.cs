namespace DistChat.Node.Domain.Friends;

public record FriendRequest(
    Guid FromUserId,
    Guid ToUserId
)
{
    public static FriendRequest Create(Guid fromUserId, Guid toUserId) 
        => new(fromUserId, toUserId);
}