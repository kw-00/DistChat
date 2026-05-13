namespace DistChat.Node.Domain.Chat;

public record Membership(
    Guid UserId,
    Guid RoomId,
    Role Role
)
{
    public static Membership Create(Guid userId, Guid roomId, Role role) 
        => new(userId, roomId, role);
    public Membership ChangeRole(Role role) => this with { Role = role };
}
