using Microsoft.AspNetCore.SignalR;

namespace DistChat.Node.Infrastructure.RealtimeHub;

public class RealtimeHub(
    CommandDispatcher dispatcher,
    UserConnectionTracker connectionTracker
) : Hub
{

    public async Task<object> Execute(
        string groupKey,
        string commandKey,
        params object[] args
    )
    {
        var commandEnvelope = new CommandInvocation(args, Context.ConnectionId);
        return await dispatcher.ExecuteAsync(groupKey, commandKey, commandEnvelope);
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            await connectionTracker.InvokeUserConnectedAsync(Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        catch (Exception)
        {
            Context.Abort();
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await connectionTracker.InvokeUserDisconnectedAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}