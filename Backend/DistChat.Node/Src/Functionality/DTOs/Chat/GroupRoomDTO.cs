using DistChat.Node.Functionality.Database.Chat;

namespace DistChat.Node.Functionality.DTOs.Chat;

public class GroupRoomDTO : RoomDTO
{
    public string Name { get; }
    public override string Type => RoomTable.Type.Dm;

    public GroupRoomDTO(Guid id, string name)
        : base (id)
    {
        Name = name;
    }
}

