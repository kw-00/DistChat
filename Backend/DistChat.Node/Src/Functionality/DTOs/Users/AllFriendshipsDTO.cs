namespace DistChat.Node.Functionality.DTOs.Users;

public class AllFriendshipsDTO
{
    public IReadOnlyList<PublicUserDTO> Friends { get; }
    public IReadOnlyList<PublicUserDTO> RequestingFriendship { get; }

    public AllFriendshipsDTO(
        IEnumerable<PublicUserDTO> friends, 
        IEnumerable<PublicUserDTO> requestingFriendship
    )
    {
        Friends = [.. friends];
        RequestingFriendship = [.. requestingFriendship];
    }
}