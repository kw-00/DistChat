using DistChat.Node.Functionality.Database.Chat;

namespace DistChat.Node.Functionality.Application.Chat;

public interface IChatOperations
{

    Task CreateRoomAsync(
        string connectionId, IEnumerable<Guid> memberIds, string name
    );

    Task<IReadOnlyList<Room>> GetRoomsAsync(string connectionId);

    Task AddUserAsync(string connectionId, Guid roomId, Guid userToAddId);

    Task RemoveUserAsync(string connectionId, Guid roomId, Guid userToRemoveId);

    Task DeleteRoomAsync(string connectionId, Guid roomId);

    Task LeaveRoomAsync(string connectionId, Guid roomId);

    Task SendMessageAsync(string connectionId, Guid roomId, string content);

    Task<IReadOnlyList<Message>> FocusRoomAsync(string connectionId, Guid? roomId);

    Task<IReadOnlyList<Message>> GoOlderAsync(
        string connectionId, Guid oldestMessageOnClientId
    );

    Task<IReadOnlyList<Message>> GoNewerAsync(
        string connectionId, Guid newestMessageOnClientId
    );

    Task HandleConnectedAsync(string connectionId, Guid userId);

    Task HandleDisconnectedAsync(string connectionId, Guid userId);
}