using DistChat.Node.Exceptions;

namespace DistChat.Node.Domain.Chat;

public class DmConversionException(Room room, RoomType attemptedType) 
: DistChatException(GetMessage(room, attemptedType))
{
    private static string GetMessage(Room room, RoomType attemptedType)
    {
        if (room.Type == RoomType.Dm)
            return 
                "Cannot convert a DM room to another type of room."
                + $"Room data: {room}.";

        return 
            "Cannot convert a non-DM room to a DM room."
            + $"Room data: {room}.";
    }
}