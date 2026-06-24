using DistChat.Node.Functionality.Database.Chat;

namespace DistChat.Node.Functionality.DTOs.Chat;

public class DmRoomDTO : RoomDTO
{
    public override string Type => RoomTable.Type.Group;

    public DmRoomDTO(Guid id)
        : base(id)
    { }
}

