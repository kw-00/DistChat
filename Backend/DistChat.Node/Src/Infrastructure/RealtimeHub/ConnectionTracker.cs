using System.Collections;
using System.Collections.Concurrent;
using DistChat.Node.Exceptions;
using Pipelines.Sockets.Unofficial;

namespace DistChat.Node.Infrastructure.RealtimeHub;

public class ConnectionTracker
{
    public event Func<TrackedConnectionInfo, Task>? UserConnected;
    public event Func<TrackedConnectionInfo, Task>? UserDisconnected;
    private readonly ConcurrentDictionary<string, Guid> _connectionos = new();

    public Guid GetUserId(string connectionId)
        => _connectionos[connectionId];


    public Guid? TryGetUserId(string connectionId)
        =>_connectionos.TryGetValue(connectionId, out var userId) ? userId : null;
    

    public Guid Put(string connectionId, Guid userId)
        => _connectionos.AddOrUpdate(connectionId, userId, (k, v) => userId);

    public void Remove(string connectionId) 
        => _connectionos.TryRemove(connectionId, out _);

    public async Task InvokeUserConnectedAsync(string connectionId)
    {
        await InvokeHookAsync(UserConnected, connectionId);
    }


    public async Task InvokeUserDisconnectedAsync(string connectionId)
    {
        await InvokeHookAsync(UserDisconnected, connectionId);
    }

    private async Task InvokeHookAsync(Func<TrackedConnectionInfo, Task>? hook, string connectionId)
    {
        try
        {
            var userId = _connectionos[connectionId];
            var connectionInfo = new TrackedConnectionInfo(connectionId, userId);
            if (hook is null) return;
            var tasks = new Task[hook.GetInvocationList().Length];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = hook(connectionInfo);
            }
            await Task.WhenAll(tasks);
        }
        catch (KeyNotFoundException ex)
        {
            throw new DistChatException(
                "An event was invoked for a connection,"
                + " yet that connection does not exist in tracker.", 
                ex
            );
        }
    }
}

public class TrackedConnectionInfo
{
    public string ConnectionId { get; }
    public Guid UserId { get; }

    public TrackedConnectionInfo(string connectionId, Guid userId)
    {
        ConnectionId = connectionId;
        UserId = userId;
    }
}