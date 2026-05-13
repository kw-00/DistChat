namespace DistChat.Node.Domain.Users;

public record User(
    Guid Id,
    string Name,
    string Email,
    string PasswordHash
)
{
    public static User Create(string name, string email, string passwordHash) 
        => new(Guid.NewGuid(), name, email, passwordHash);

    public override string ToString()
        => $"User {{ Id = {Id}, Name = {Name}, Email = {Email} }}";
}
