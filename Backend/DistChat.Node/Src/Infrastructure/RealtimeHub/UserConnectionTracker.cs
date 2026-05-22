using System.Collections;
using System.Collections.Concurrent;
using DistChat.Node.Exceptions;
using DistChat.Node.Functionality.Exceptions.Users;
using DistChat.Node.Infrastructure.Concurrency;
using Pipelines.Sockets.Unofficial;

namespace DistChat.Node.Infrastructure.RealtimeHub;

public class UserConnectionTracker
{
    public event Func<TrackedConnectionInfo, Task>? UserConnected;
    public event Func<TrackedConnectionInfo, Task>? UserDisconnected;
    private readonly ConcurrentDictionary<string, Guid> _connections = new();
    private readonly ConcurrentDictionary<Guid, ICollection<string>> 
        _users = new();

    private readonly PartitionedLock<Guid> _locks = new(10_000);

    

    public Guid GetUserId(string connectionId)
    {
        try
        {
            return _connections[connectionId];
        }
        catch (KeyNotFoundException ex)
        {
            throw new UserNotFoundException(
                "No user found associated with" 
                + $" connection ID of \"{connectionId}\".",
                ex
            );
        }
    }


    public Guid? TryGetUserId(string connectionId)
        =>_connections.TryGetValue(connectionId, out var userId) ? userId : null;

    public IReadOnlyCollection<string> GetConnections(Guid userId)
    {
        return _users.TryGetValue(userId, out var connections) 
            ? [.. connections] 
            : [];
    }
    

    public void Put(string connectionId, Guid userId)
    {
        lock (_locks.Get(userId))
        {
            if (_connections.ContainsKey(connectionId))
                throw new ArgumentException(
                    $"Connection with ID of \"{connectionId}\" is already registered."
                );
            _connections[connectionId] = userId;
            _users.AddOrUpdate(
                userId, [connectionId], (_, cs) =>
                {
                    cs.Add(connectionId);
                    return cs;
                }
            );
        }
    }

    public void RemoveConnection(string connectionId)
    {
        var userIdFound = _connections.TryGetValue(connectionId, out var userId);
        if (!userIdFound) return;
        lock (_locks.Get(userId))
        {
            var userIdFoundAfterLocking = _connections.TryGetValue(
                connectionId, out var userIdAfterLocking
            );
            if (!userIdFoundAfterLocking || userId != userIdAfterLocking) return;
            _connections.TryRemove(connectionId, out var _);
            _users.AddOrUpdate(
                userId, [connectionId], (_, cs) =>
                {
                    cs.Remove(connectionId);
                    return cs;
                }
            );
            if (_users[userId].Count == 0) _users.TryRemove(userId, out var _);
        }
    }


    public async Task InvokeUserConnectedAsync(string connectionId)
    {
        await InvokeHookAsync(UserConnected, connectionId);
        RemoveConnection(connectionId);
    }


    public async Task InvokeUserDisconnectedAsync(string connectionId)
    {
        await InvokeHookAsync(UserDisconnected, connectionId);
        RemoveConnection(connectionId);
    }

    private async Task InvokeHookAsync(Func<TrackedConnectionInfo, Task>? hook, string connectionId)
    {
        try
        {
            var userId = _connections[connectionId];
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