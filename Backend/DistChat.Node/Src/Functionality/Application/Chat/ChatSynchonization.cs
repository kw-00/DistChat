namespace DistChat.Node.Functionality.Application.Chat;

public class ChatSynchronization(
    RoomSemaphores roomSemaphores,
    MessageFeedSemaphores connectionSemaphores
)
{

    public Task WaitRoomAsync(Guid roomId) 
        => GetRoomSemaphore(roomId).WaitAsync();

    public Task WaitConnectionAsync(string connectionId) 
        => GetConnectionSemaphore(connectionId).WaitAsync();

    public async Task WaitRoomAndConnectionAsync(Guid roomId, string connectionId)
    {
        await GetRoomSemaphore(roomId).WaitAsync();
        await GetConnectionSemaphore(connectionId).WaitAsync();
    }

    public void ReleaseRoom(Guid roomId) 
        => GetRoomSemaphore(roomId).Release();

    public void ReleaseConnection(string connectionId) 
        => GetConnectionSemaphore(connectionId).Release();

    public void ReleaseRoomAndConnection(Guid roomId, string connectionId)
    {
        GetRoomSemaphore(roomId).Release();
        GetConnectionSemaphore(connectionId).Release();
    }

    private SemaphoreSlim GetRoomSemaphore(Guid roomId) 
        => roomSemaphores.Get(roomId);
    private SemaphoreSlim GetConnectionSemaphore(string connectionId) 
        => connectionSemaphores.Get(connectionId);
}