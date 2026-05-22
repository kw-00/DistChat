namespace DistChat.Node.Infrastructure.RealtimeHub;

public class CommandGroup
{
    private readonly Dictionary<string, Func<CommandInvocation, Task<object>>> 
        _handlers = [];

    public void RegisterCommand(string key, Func<CommandInvocation, Task<object>> handler)
    {
        try
        {
            _handlers.Add(key, handler);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException(
                $"Command \"{key}\" is already registered in command group."
            );
        }
    }

    public void RegisterCommand(string key, Func<CommandInvocation, Task> handler)
    {
        try
        {
            async Task<object> fullHandler(CommandInvocation invocation)
            {
                await handler(invocation);
                return new object();
            }
            _handlers.Add(key, fullHandler);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException(
                $"Command \"{key}\" is already registered in command group."
            );
        }
    }

    public async Task<object> ExecuteAsync(string key, CommandInvocation invocation)
    {
        var handler = _handlers.GetValueOrDefault(key) 
            ?? throw new KeyNotFoundException(
                $"Command \"{key}\" is not registered in command group."
            );
        return await handler(invocation);
    }
}

