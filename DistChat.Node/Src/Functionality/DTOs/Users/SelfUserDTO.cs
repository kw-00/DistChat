using DistChat.Node.Functionality.Database.Users;

namespace DistChat.Node.Functionality.DTOs.Users;

public class SelfUserDTO
{
    public Guid Id { get; }
    public string Login { get; }
    public string Email { get; }

    public SelfUserDTO(User user)
    {
        Id = user.Id;
        Login = user.Login;
        Email = user.Email;
    }
}