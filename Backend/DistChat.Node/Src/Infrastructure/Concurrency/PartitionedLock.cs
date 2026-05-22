using System.Security.Cryptography;

namespace DistChat.Node.Infrastructure.Concurrency;

public class PartitionedLock<TPartitionKey>(int partitionCount) 
    : PartitionedAccessor<TPartitionKey, Lock>(partitionCount, () => new Lock())
    where TPartitionKey : notnull
{ }