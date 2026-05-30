using DistChat.Node.Functionality.Database.Users;

namespace DistChat.Node.Functionality.DTOs.Users;

public class PublicUserDTO
{
    public Guid Id { get; }
    public string Login { get; }

    public PublicUserDTO(User user)
    {
        Id = user.Id;
        Login = user.Login;
    }
}