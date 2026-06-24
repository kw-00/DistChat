namespace DistChat.Node.Functionality.Database.Users;

public class User
{
    public Guid Id { get; }
    public string Login { get; }
    public string Email { get; }
    public string PasswordHash { get; }

    public User(Guid id, string login, string email, string passwordHash)
    {
        Id = id;
        Login = login;
        Email = email;
        PasswordHash = passwordHash;
    }
}
