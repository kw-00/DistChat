using DistChat.Node.Functionality.DTOs.Chat;
using DistChat.Node.Functionality.DTOs.Users;

namespace DistChat.Node.Functionality.Database.Chat; 

public interface IRoomDbService 
{
    Task<GroupRoomDTO> CreateGroupRoomAsync(
        Guid id, string name, Guid creatorId, IEnumerable<Guid> memberIds
    );
    Task<DmRoomDTO> ConnectUsersAsync(Guid newDmId, Guid userAId, Guid userBId);

    Task<RoomDTO> GetRoomAsync(Guid roomId);
    Task<IReadOnlyList<RoomDTO>> GetRoomsAsync(Guid userId);
    Task<IReadOnlyList<PublicUserDTO>> GetUsersAsync(Guid roomId);
    Task AddUserAsync(
        Guid initiatingUserId, Guid userToBeAddedId, Guid groupRoomId
    );
    Task RemoveUserAsync(
        Guid initiatingUserId, Guid userToBeRemovedId, Guid groupRoomId
    );

    Task RemoveUserAsync(Guid userId, Guid groupRoomId);

    Task DeleteGroupRoomAsync(Guid initiatingUserId, Guid groupRoomId);

    Task<bool> IsUserInRoomAsync(Guid userId, Guid roomId);

}