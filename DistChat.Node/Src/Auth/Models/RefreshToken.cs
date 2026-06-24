namespace DistChat.Node.Auth.Models;

public class RefreshToken
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public bool IsUsed { get; }
    public DateTimeOffset ExpiresAt { get; }

    public RefreshToken(Guid id, Guid userId, bool isUsed, DateTimeOffset expiresAt)
    {
        Id = id;
        UserId = userId;
        IsUsed = isUsed;
        ExpiresAt = expiresAt;
    }
}