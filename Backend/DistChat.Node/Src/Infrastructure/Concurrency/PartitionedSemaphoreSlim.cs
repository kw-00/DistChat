using System.Security.Cryptography;

namespace DistChat.Node.Infrastructure.Concurrency;

public class PartitionedSemaphoreSlim<TPartitionKey>
    : PartitionedAccessor<TPartitionKey, SemaphoreSlim>
    where TPartitionKey : notnull
{
    public PartitionedSemaphoreSlim(int partitionCount, int initialLimit)
        : base(partitionCount, () => new SemaphoreSlim(initialLimit)) { }

    public PartitionedSemaphoreSlim(int partitionCount, int initialLimit, int maxLimit)
        : base(partitionCount, () => new SemaphoreSlim(initialLimit, maxLimit)) { }
}