namespace DistChat.Node.Auth.Models;

public class PendingRegistration
{
    public Guid Id { get; }
    public string Login { get; }
    public string Email { get; }

    public string PasswordHash { get; }

    public DateTimeOffset ExpiresAt { get; }

    public PendingRegistration(
        Guid id,
        string login,
        string email,
        string passwordhash,
         DateTimeOffset expiresat
    )
    {
        Id = id;
        Login = login;
        Email = email;
        PasswordHash = passwordhash;
        ExpiresAt = expiresat;
    }
}