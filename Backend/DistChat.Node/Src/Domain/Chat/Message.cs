namespace DistChat.Node.Domain.Chat;

public record Message(
    Guid Id,
    Guid UserId,
    Guid RoomId,
    string Content,
    DateTime CreatedAt
)
{
    public static Message Create(Guid id, Guid userId, Guid roomId, string content) 
        => new(id, userId, roomId, content, DateTime.UtcNow);
}
