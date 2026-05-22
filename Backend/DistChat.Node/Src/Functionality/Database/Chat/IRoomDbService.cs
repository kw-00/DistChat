using DistChat.Node.Functionality.DTOs.Users;

namespace DistChat.Node.Functionality.Database.Chat; 

public interface IRoomDbService 
{
    Task<GroupRoom> CreateGroupRoomAsync(
        Guid id, string name, Guid creatorId, IEnumerable<Guid> memberIds
    );
    Task<DmRoom> ConnectUsersAsync(Guid newDmId, Guid userAId, Guid userBId);

    Task<Room> GetRoomAsync(Guid roomId);
    Task<IReadOnlyList<Room>> GetRoomsAsync(Guid userId);
    Task<IReadOnlyList<PublicUserDTO>> GetUsersAsync(Guid roomId);
    Task AddUserAsync(Guid initiatingUserId, Guid userToBeAddedId, Guid roomId);
    Task RemoveUserAsync(Guid initiatingUserId, Guid userToBeRemovedId, Guid roomId);

    Task RemoveUserAsync(Guid userId, Guid roomId);

    Task DeleteGroupRoomAsync(Guid initiatingUserId, Guid roomId);

    Task<bool> IsUserInRoomAsync(Guid userId, Guid roomId);

}