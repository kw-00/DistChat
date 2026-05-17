using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace DistChat.Node.Infrastructure.EventManagement;

public class EventManager : IEventManager
{
    private readonly IHubContext _hubContext;
    private readonly ISubscriber _subscriber;

    private readonly SubscriptionCounter _subscriptionCounter;
    private readonly JsonSerializerOptions _jsonOptions;


    public EventManager(
        IHubContext hubContext,
        SubscriptionCounter subscriptionCounter,
        IConnectionMultiplexer redis,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        _hubContext = hubContext;
        _subscriber = redis.GetSubscriber();
        _subscriptionCounter = subscriptionCounter;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }


    public async Task PublishAsync(Event @event)
    {
        var channel = RedisChannel.Literal(@event.Address.Serialize());
        var message = JsonSerializer.Serialize(@event, _jsonOptions);
        await _subscriber.PublishAsync(channel, message);
    }

    public async Task SubscribeAsync(string connectionId, EventAddress eventAddress)
    { 
        var address = eventAddress.Serialize();
        await _hubContext.Groups.AddToGroupAsync(connectionId, address);
        lock (_subscriptionCounter.GetLock(address))
        {
            if (_subscriptionCounter.TryIncrement(address)) return;
            _subscriber.Subscribe(RedisChannel.Literal(address), OnRedisEvent);
        }
    }

    public async Task UnsubscribeAsync(string connectionId, EventAddress eventAddress)
    {
        var address = eventAddress.Serialize();
        await _hubContext.Groups.RemoveFromGroupAsync(connectionId, address);
        lock (_subscriptionCounter.GetLock(address))
        {
            if (_subscriptionCounter.TryDecrement(address)) return;
            _subscriber.Unsubscribe(RedisChannel.Literal(address), OnRedisEvent);
        }
    }

    private void OnRedisEvent(RedisChannel channel, RedisValue value)
    {
        var json = value.ToString();
        var evt = JsonSerializer.Deserialize<Event>(json, _jsonOptions)!;
        _hubContext.Clients.Group(evt.Address.Serialize()).SendAsync(evt.Address.Topic, evt);
    }
}
