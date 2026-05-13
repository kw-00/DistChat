namespace DistChat.Node.Domain.Friends;

public record Friendship(
    Guid FirstUserId,
    Guid SecondUserId
)
{
    public static Friendship Create(Guid userAId, Guid userBId)
    {
        if (userAId < userBId) return new(userAId, userBId);
        return new(userBId, userAId);
    }
}