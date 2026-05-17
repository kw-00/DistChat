namespace DistChat.Node.Functionality.Database.Chat; 

public interface IRoomDbService 
{
    Task<GroupRoom> CreateGroupRoomAsync(
        Guid creatorId, IEnumerable<Guid> memberIds, string name
    );
    Task<DmRoom> ConnectUsersAsync(Guid userAId, Guid userBId);
    Task<IReadOnlyList<Room>> GetRoomsAsync(Guid userId);
    Task AddUserAsync(Guid initiatingUserId, Guid userToBeAddedId, Guid roomId);
    Task RemoveUserAsync(Guid initiatingUserId, Guid userToBeRemovedId, Guid roomId);

    Task RemoveUserAsync(Guid userId, Guid roomId);

}