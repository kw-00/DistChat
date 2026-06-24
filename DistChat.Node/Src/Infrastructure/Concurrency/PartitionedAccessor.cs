namespace DistChat.Node.Infrastructure.Concurrency;

public class PartitionedAccessor<TPartitionKey, TValue>
    where TPartitionKey : notnull
{
    private readonly int _partitionCount;
    private readonly TValue[] _values;

    private readonly Func<TPartitionKey, object> _keySelector;


    public PartitionedAccessor(
        int partitionCount,
        Func<TValue> factory,
        Func<TPartitionKey, object>? keySelector = null
    )
    {
        _partitionCount = partitionCount;
        _values = new TValue[partitionCount];
        for (var i = 0; i < partitionCount; i++)
            _values[i] = factory();

        _keySelector = keySelector ?? DefaultKeySelector;
    }

    public TValue Get(TPartitionKey o)
    {
        var partition = (_keySelector(o).GetHashCode() & 0x7fffffff) % _partitionCount;
        return _values[partition];
    }

    private static object DefaultKeySelector(TPartitionKey o) => o;
}