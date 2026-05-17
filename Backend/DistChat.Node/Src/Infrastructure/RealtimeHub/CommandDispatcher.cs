namespace DistChat.Node.Infrastructure.RealtimeHub;

public class CommandDispatcher
{
    private readonly Dictionary<string, CommandGroup> _commandGroups = [];
    public void RegisterGroup(string key, CommandGroup group)
    {
        try
        {
            _commandGroups.Add(key, group);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException(
                $"Command group \"{key}\" is already registered in dispatcher."
            );
        }
    }

    public async Task<object> ExecuteAsync(
         string groupKey, string commandKey, CommandInvocation invocation
    )
    {
        var group = _commandGroups.GetValueOrDefault(groupKey) 
            ?? throw new KeyNotFoundException(
                $"Command group \"{groupKey}\" is not registered in dispatcher."
            );
        return await group.ExecuteAsync(commandKey, invocation);
    }
}

