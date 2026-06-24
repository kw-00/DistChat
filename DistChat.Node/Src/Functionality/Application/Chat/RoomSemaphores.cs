using DistChat.Node.Infrastructure.Concurrency;

namespace DistChat.Node.Functionality.Application.Chat;

public class RoomSemaphores() : PartitionedSemaphoreSlim<Guid>(10_000, 1, 1);