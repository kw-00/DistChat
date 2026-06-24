namespace DistChat.Node.Functionality.DTOs.Users;

public class FriendRequestDTO
{
    public PublicUserDTO RequestingUser { get; }
    public PublicUserDTO TargetUser { get; }

    public FriendRequestDTO(PublicUserDTO requestingUser, PublicUserDTO targetUser)
    {
        RequestingUser = requestingUser;
        TargetUser = targetUser;
    }
}