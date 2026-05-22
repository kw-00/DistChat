using DistChat.Node.Functionality.DTOs.Users;

namespace DistChat.Node.Functionality.DTOs.Chat;

public class UserJoinedtDTO
{
    public PublicUserDTO User { get; }
    public Guid RoomId { get; }

    public UserJoinedtDTO(PublicUserDTO user, Guid roomId)
    {
        User = user;
        RoomId = roomId;
    }
}