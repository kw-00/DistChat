using DistChat.Node.Functionality.Database.Chat;
using DistChat.Node.Functionality.Models.Chat;

namespace DistChat.Node.Functionality.DTOs.Chat;

public abstract class RoomDTO
{
    public Guid Id { get; }
    public abstract string Type { get; }

    public RoomDTO(Guid id)
    {
        Id = id;
    }

    public static RoomDTO FromRoom(Room room)
    {
        if (room.Type == RoomTable.Type.Dm)
        {
            return new DmRoomDTO(room.Id);
        }
        if (room.Name is null)
            throw new ArgumentException(
                "Room name cannot only be null for DMs."
            );
        return new GroupRoomDTO(room.Id, room.Name);
    }
}
