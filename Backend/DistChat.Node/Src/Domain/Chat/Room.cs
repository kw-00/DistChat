namespace DistChat.Node.Domain.Chat;

public record Room(
    Guid Id,
    string Name,
    RoomType Type
)
{
    public static Room Create(string name, RoomType type) 
        => new(Guid.NewGuid(), name, type);

    public Room ChangeType(RoomType type)
    {
        if (Type == RoomType.Dm || type == RoomType.Dm)
            throw new DmConversionException(this, type);
        return this with { Type = type };
    }
}

