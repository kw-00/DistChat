using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;

namespace DistChat.Node.Infrastructure.RealtimeHub;

public class CommandInvocation(object[] args, string connectionId)
{
    public object[] Args { get; } = args;
    public string ConnectionId { get; } = connectionId;
}