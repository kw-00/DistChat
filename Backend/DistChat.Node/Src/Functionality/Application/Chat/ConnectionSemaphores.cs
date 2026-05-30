using DistChat.Node.Infrastructure.Concurrency;

namespace DistChat.Node.Functionality.Application.Chat;

public class MessageFeedSemaphores()
    : PartitionedSemaphoreSlim<string>(10_000, 1, 1);