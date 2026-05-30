namespace DistChat.Node.Functionality.DTOs.Chat;

public class UserLeftDTO
{
    public Guid UserId { get; }
    public Guid RoomId { get; }

    public UserLeftDTO(Guid userId, Guid roomId)
    {
        UserId = userId;
        RoomId = roomId;
    }
}