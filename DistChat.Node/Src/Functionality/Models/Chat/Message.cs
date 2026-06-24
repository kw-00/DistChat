namespace DistChat.Node.Functionality.Database.Chat;

public class Message
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid RoomId { get; }
    public string Content { get; }
    public DateTimeOffset CreatedAt { get; }

    public Message(
        Guid id, Guid userId, Guid roomId, string content, DateTimeOffset createdAt
    )
    {
        Id = id;
        UserId = userId;
        RoomId = roomId;
        Content = content;
        CreatedAt = createdAt;
    }
}

