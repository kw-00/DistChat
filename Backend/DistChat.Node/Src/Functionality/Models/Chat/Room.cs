using DistChat.Node.Functionality.Database.Chat;

namespace DistChat.Node.Functionality.Models.Chat;

public abstract class Room
{
    public Guid Id { get; }
    public string? Name { get; }
    public string Type { get; }

    public Room(Guid id, string? name, string type)
    {
        if (type != RoomTable.Type.Dm)
            throw new ArgumentException(
                "Null room name is only allowed for DMs."
            );
        Id = id;
        Name = name;
        Type = type;
    }
}

