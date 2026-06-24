namespace DistChat.Node.Functionality.Database.Chat;

public class Membership
{
    public Guid UserId { get; }
    public Guid RoomId { get; }
    public Role Role { get; }

    public Membership(Guid userId, Guid roomId, string role)
    {
        UserId = userId;
        RoomId = roomId;
        Role = role.ToLower() switch
        {
            "elder" => Role.Elder,
            "owner" => Role.Owner,
            _ => Role.Member
        };
    }
}

public enum Role
{
    Member,
    Elder,
    Owner
}

