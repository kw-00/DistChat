using System.Collections.Concurrent;
using System.Text.Json;
using DistChat.Node.Infrastructure.Concurrency;
using Microsoft.AspNetCore.SignalR;
using NATS.Net;

namespace DistChat.Node.Infrastructure.EventManagement;

public class EventManager : IEventManager
{
    private readonly IHubContext _hubContext;
    private readonly NatsClient _nats;
    private readonly ILogger<EventManager> _logger;
    private readonly JsonSerializerOptions _jsonOptions;


    private readonly ConcurrentDictionary<string, CancellationTokenSource>
        _subCancellation = new();

    private readonly PartitionedLock<string> _subCancellationLocks = new(10000);

    public EventManager(
        IHubContext hubContext,
        NatsClient nats,
        ILogger<EventManager> logger,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        _hubContext = hubContext;
        _nats = nats;
        _logger = logger;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }


    public async Task PublishAsync(Event @event)
    {
        var channel = @event.Address.Serialize();
        var message = JsonSerializer.Serialize(@event, _jsonOptions);
        await _nats.PublishAsync(channel, message);
    }

    public Task StartConsumptionAsync(string connectionId, EventAddress eventAddress)
    {
        var subscriptionId = CreateSubscriptionId(connectionId, eventAddress);
        var cancellation = new CancellationTokenSource();
        var address = eventAddress.Serialize();

        async Task Listen()
        {
            await foreach (
                var message in _nats
                    .SubscribeAsync<string>(address)
                    .WithCancellation(cancellation.Token)
            )
            {
                var data = message.Data;
                if (data is null)
                {
                    _logger.LogError("NATS message is unexpectedly null.");
                    continue;
                }
                var evt = JsonSerializer.Deserialize<Event>(data, _jsonOptions);
                if (evt is null)
                {
                    _logger.LogError("NATS message was not deserialized successfully.");
                    continue;
                }
                await _hubContext.Clients.Client(connectionId).SendAsync(
                    evt.Address.Topic,
                    evt.Data
                );
            }
        }

        lock (_subCancellationLocks.Get(subscriptionId))
        {
            if (_subCancellation.ContainsKey(subscriptionId))
                return Task.CompletedTask;

            _subCancellation[subscriptionId] = cancellation;
            _ = Listen();
            return Task.CompletedTask;
        }
    }

    public async Task StopConsumptionAsync(string connectionId, EventAddress eventAddress)
    {
        var subId = CreateSubscriptionId(connectionId, eventAddress);
        CancellationTokenSource? cancellation;
        lock (_subCancellationLocks.Get(subId))
        {
            _subCancellation.TryRemove(subId, out cancellation);
        }
        if (cancellation is null) return;
        await cancellation.CancelAsync();
    }

    private string CreateSubscriptionId(string connectionId, EventAddress eventAddress)
        => JsonSerializer.Serialize(new { connectionId, eventAddress });

}
