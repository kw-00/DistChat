using System.Collections.Concurrent;

namespace DistChat.Node.Functionality.Application.Chat;

public class RoomFocusTracker
{
    private ConcurrentDictionary<string, Guid> _roomFocus = new();

    public Guid? TryGetRoomFocus(string connectionId)
    {
        var found = _roomFocus.TryGetValue(connectionId, out var roomId);
        return found ? roomId : null;
    }

    public Guid GetRoomFocus(string connectionId)
    {
        try
        {
            return _roomFocus[connectionId];
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException("No room focused.", ex);
        }
    }

    public void SetRoomFocus(string connectionId, Guid roomId)
    {
        _roomFocus.AddOrUpdate(connectionId, roomId, (_, _) => roomId);
    }

    public void ClearRoomFocus(string connectionId)
    {
        _roomFocus.TryRemove(connectionId, out _);
    }
}