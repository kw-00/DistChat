using System.Collections.Concurrent;

namespace DistChat.Node.Infrastructure.EventManagement;

public class SubscriptionCounter
{
    private readonly int _partitions;
    private readonly Lock[] _locks;
    private readonly ConcurrentDictionary<string, int> _counts = new();


    public SubscriptionCounter(int partitions)
    {
        _partitions = partitions;
        _locks = new Lock[partitions];
        for (var i = 0; i < partitions; i++)
        {
            _locks[i] = new Lock();
        }
    }

    public bool TryIncrement(string address)
    {
        lock (GetLock(address))
        {
            int count =_counts.AddOrUpdate(address, 1, (k, v) => v + 1);
            if (count == 1) return false;
            return true;
        }
    }


    public bool TryDecrement(string address)
    {
        lock (GetLock(address))
        {
            var count =_counts.AddOrUpdate(address, 0, (k, v) => Math.Max(0, v - 1));
            if (count == 0)
            {
                _counts.TryRemove(address, out _);
                return false;
            }

            return true;
        }
    }

    public Lock GetLock(string address)
    {
        var partition = GetPartition(address);
        return _locks[partition];
    }


    private int GetPartition(string address)
    {
        return address.GetHashCode() % _partitions;
    }
}