using System.Diagnostics.CodeAnalysis;

namespace DistChat.Node.Infrastructure.EventManagement;

public class EventPayload
{
    [MemberNotNullWhen(true, nameof(Data))]
    [MemberNotNullWhen(false, nameof(Exception))]
    public bool Success => Exception == null;

    public Exception? Exception { get; init; }
    public object? Data { get; init; }

    public static EventPayload SuccessPayload(object data) => new() { 
        Data = data
    };

    public static EventPayload SuccessPayload() => new()
    {
        Data = new object()
    };

    public static EventPayload FailurePayload(Exception exception) => new() { 
        Exception = exception
    };
}