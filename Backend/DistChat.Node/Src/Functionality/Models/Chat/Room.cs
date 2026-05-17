namespace DistChat.Node.Functionality.Database.Chat;

public abstract class Room
{
    public Guid Id { get; }
    public string Name { get; }
    public abstract RoomType Type { get; }

    public Room(Guid id, string name) => (Id, Name) = (id, name);
}

public class GroupRoom : Room
{
    public override RoomType Type => RoomType.Group;

    public GroupRoom(Guid id, string name) : base(id, name) { }
}

public class DmRoom : Room
{
    public override RoomType Type => RoomType.Dm;

    public DmRoom(Guid id, string name) : base(id, name) { }
}



public enum RoomType
{
    Group,
    Dm
}

