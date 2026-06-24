using DistChat.Node.Functionality.Database.Chat;
using DistChat.Node.Functionality.Database.Users;
using DistChat.Node.Functionality.DTOs.Chat;
using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Functionality.Exceptions.Chat;
using DistChat.Node.Functionality.Exceptions.Users;
using DistChat.Node.Functionality.Options.Chat;
using DistChat.Node.Infrastructure.RealtimeHub;
using Microsoft.Extensions.Options;

namespace DistChat.Node.Functionality.Application.Chat;

public class ChatOperations(
        IMessageDbService messageDbService,
        IRoomDbService roomDbService,
        IUserDbService userDbService,

        MessageReceived messageReceived,
        RemovedFromRoom removedFromRoom,
        AddedToRoom addedToRoom,
        UserJoined userJoined,
        UserLeft userLeft,

        UserConnectionTracker connectionTracker,
        RoomFocusTracker roomFocusTracker,
        ChatSynchronization synchronization,

        IOptions<MessageOptions> options
) : IChatOperations
{
    public async Task CreateRoomAsync(
        string connectionId, IEnumerable<Guid> memberIds, string name
    )
    {
        var roomId = Guid.NewGuid();
        await synchronization.WaitRoomAsync(roomId);
        try
        {
            var userId = connectionTracker.GetUserId(connectionId);
            IReadOnlyList<Guid> memberIdsList = [.. memberIds];
            var room = await roomDbService.CreateGroupRoomAsync(
                roomId, name, userId, memberIdsList
            );
            await Task.WhenAll(
                memberIdsList.Select(
                    (memberId) => addedToRoom.PublishAsync(memberId, room)
                )
            );
        }
        finally
        {
            synchronization.ReleaseRoom(roomId);
        }
    }

    public async Task<IReadOnlyList<RoomDTO>> GetRoomsAsync(string connectionId)
    {
        var userId = connectionTracker.GetUserId(connectionId);
        return await roomDbService.GetRoomsAsync(userId);
    }

    public async Task AddUserAsync(
        string connectionId, Guid roomId, Guid userToAddId
    )
    {
        await synchronization.WaitRoomAsync(roomId);
        try
        {
            var userId = connectionTracker.GetUserId(connectionId);
            await roomDbService.AddUserAsync(userId, roomId, userToAddId);
            var room = await roomDbService.GetGroupRoomAsync(roomId);
            var addedUser = await userDbService.GetAsync(userToAddId)
                ?? throw new UserNotFoundException(
                    $"User with ID of \"{userToAddId}\" disappeared from database."
                );
            var userJoinedDto = new UserJoinedtDTO(
                new PublicUserDTO(addedUser), roomId
            );
            await Task.WhenAll(
                addedToRoom.PublishAsync(userToAddId, room),
                userJoined.PublishAsync(roomId, userJoinedDto)
            );
            await SubscribeToRoomAsync(roomId, userToAddId);
        }
        finally
        {
            synchronization.ReleaseRoom(roomId);
        }
    }

    public async Task RemoveUserAsync(
        string connectionId, Guid roomId, Guid userToRemoveId
    )
    {
        await synchronization.WaitRoomAndConnectionAsync(roomId, connectionId);
        try
        {
            var userId = connectionTracker.GetUserId(connectionId);
            await roomDbService.RemoveUserAsync(userId, roomId, userToRemoveId);
            await UnsubscribeFromRoomAsync(roomId, userToRemoveId);
            var userLeftDto = new UserLeftDTO(userToRemoveId, roomId);
            await Task.WhenAll(
                userLeft.PublishAsync(roomId, userLeftDto),
                removedFromRoom.PublishAsync(userToRemoveId, roomId)
            );
        }
        finally
        {
            synchronization.ReleaseRoomAndConnection(roomId, connectionId);
        }
    }

    public async Task DeleteRoomAsync(string connectionId, Guid roomId)
    {
        await synchronization.WaitRoomAndConnectionAsync(roomId, connectionId);
        try
        {
            var userId = connectionTracker.GetUserId(connectionId);
            await roomDbService.DeleteGroupRoomAsync(userId, roomId);
            var roomMembers = await roomDbService.GetUsersAsync(roomId);
            async Task ProcessMemberAsync(Guid memberId)
            {
                await removedFromRoom.PublishAsync(memberId, roomId);
                await UnsubscribeFromRoomAsync(roomId, memberId);
            }
            ;
            await Task.WhenAll(
                roomMembers.Select(
                    (member) => ProcessMemberAsync(member.Id)
                )
            );

        }
        finally
        {
            synchronization.ReleaseRoomAndConnection(roomId, connectionId);
        }
    }

    public async Task LeaveRoomAsync(string connectionId, Guid roomId)
    {
        await synchronization.WaitRoomAndConnectionAsync(roomId, connectionId);
        try
        {
            var userId = connectionTracker.GetUserId(connectionId);
            await roomDbService.DeleteGroupRoomAsync(userId, roomId);
            var roomMembers = await roomDbService.GetUsersAsync(roomId);
            async Task ProcessMemberAsync(Guid memberId)
            {
                await removedFromRoom.PublishAsync(memberId, roomId);
                await UnsubscribeFromRoomAsync(roomId, memberId);
            }
            ;
            await Task.WhenAll(roomMembers.Select(
                (member) => ProcessMemberAsync(member.Id)
            ));

        }
        finally
        {
            synchronization.ReleaseRoomAndConnection(roomId, connectionId);
        }
    }



    public async Task<IReadOnlyList<Message>> FocusRoomAsync(
        string connectionId, Guid? roomId
    )
    {
        await synchronization.WaitConnectionAsync(connectionId);

        IReadOnlyList<Message> messages;
        try
        {
            if (roomId is null)
            {
                var focusedRoomId = roomFocusTracker.TryGetRoomFocus(connectionId);
                if (focusedRoomId is not null)
                {
                    await messageReceived.StopConsumptionAsync(
                        connectionId, focusedRoomId.Value
                    );
                    roomFocusTracker.ClearRoomFocus(
                        connectionId
                    );
                }
                messages = [];
            }
            else
            {
                var userId = connectionTracker.GetUserId(connectionId);
                if (!await roomDbService.IsUserInRoomAsync(userId, roomId.Value))
                    throw new NotInRoomException(userId, roomId.Value);

                await messageReceived.StartConsumptionAsync(connectionId, roomId.Value);
                roomFocusTracker.SetRoomFocus(connectionId, roomId.Value);
                messages = await messageDbService.GetMessagesAsync(
                    userId,
                    roomId.Value,
                    options.Value.MessageBatchSize,
                    newestFirst: true
                );
                messages = [.. messages.Reverse()];
            }
            return messages;
        }
        finally
        {
            synchronization.ReleaseConnection(connectionId);
        }
    }

    public async Task SendMessageAsync(
        string connectionId, Guid roomId, string content
    )
    {
        var userId = connectionTracker.GetUserId(connectionId);
        var message = await messageDbService.CreateAsync(userId, roomId, content);
        await messageReceived.PublishAsync(roomId, message);
    }

    public async Task<IReadOnlyList<Message>> GoOlderAsync(
        string connectionId, Guid oldestMessageOnClientId
    )
    {
        await synchronization.WaitConnectionAsync(connectionId);

        try
        {
            var userId = connectionTracker.GetUserId(connectionId);
            var roomId = roomFocusTracker.GetRoomFocus(connectionId);
            var messages = await messageDbService.GetMessagesAsync(
                userId,
                roomId,
                50,
                before: oldestMessageOnClientId,
                newestFirst: true
            );
            messages = [.. messages.Reverse()];
            await messageReceived.StopConsumptionAsync(connectionId, roomId);
            return messages;
        }
        finally
        {
            synchronization.ReleaseConnection(connectionId);
        }
    }

    public async Task<IReadOnlyList<Message>> GoNewerAsync(
        string connectionId, Guid newestMessageOnClientId
    )
    {
        await synchronization.WaitConnectionAsync(connectionId);
        try
        {
            var userId = connectionTracker.GetUserId(connectionId);
            var roomId = roomFocusTracker.GetRoomFocus(connectionId);
            var messages = await messageDbService.GetMessagesAsync(
                userId,
                roomId,
                50,
                after: newestMessageOnClientId,
                newestFirst: false
            );
            if (messages.Count < options.Value.MessageBatchSize)
            {
                await messageReceived.StartConsumptionAsync(connectionId, roomId);
                var missedMessages = await messageDbService.GetMessagesAsync(
                    userId,
                    messages[messages.Count - 1].Id,
                    options.Value.MessageBatchSize,
                    newestFirst: false
                );
                messages = [.. messages, .. missedMessages];
            }
            return messages;
        }
        finally
        {
            synchronization.ReleaseConnection(connectionId);
        }
    }


    public async Task HandleConnectedAsync(string connectionId, Guid userId)
    {
        await synchronization.WaitConnectionAsync(connectionId);
        try
        {
            var rooms = await roomDbService.GetRoomsAsync(userId);
            List<Task> tasks = [];
            tasks.Add(addedToRoom.StartConsumptionAsync(connectionId, userId));
            tasks.Add(removedFromRoom.StartConsumptionAsync(connectionId, userId));
            foreach (var room in rooms)
                tasks.Add(SubscribeToRoomAsync(room.Id, userId));

            await Task.WhenAll(tasks);
        }
        finally
        {
            synchronization.ReleaseConnection(connectionId);
        }
    }

    public async Task HandleDisconnectedAsync(string connectionId, Guid userId)
    {
        await synchronization.WaitConnectionAsync(connectionId);
        try
        {
            var rooms = await roomDbService.GetRoomsAsync(userId);
            List<Task> tasks = [];
            tasks.Add(addedToRoom.StopConsumptionAsync(connectionId, userId));
            tasks.Add(removedFromRoom.StopConsumptionAsync(connectionId, userId));
            foreach (var room in rooms)
                tasks.Add(UnsubscribeFromRoomAsync(room.Id, connectionId));

            await Task.WhenAll(tasks);
            roomFocusTracker.ClearRoomFocus(connectionId);
        }
        finally
        {
            synchronization.ReleaseConnection(connectionId);
        }
    }

    private async Task UnsubscribeFromRoomAsync(Guid roomId, Guid userId)
    {
        var userConnections = connectionTracker.GetConnections(userId);
        await Task.WhenAll(
            userConnections.Select((connectionId) =>
                UnsubscribeFromRoomAsync(roomId, connectionId)
            )
        );
    }

    private async Task UnsubscribeFromRoomAsync(Guid roomId, string connectionId)
    {
        List<Task> tasks = [];
        tasks.Add(userLeft.StopConsumptionAsync(connectionId, roomId));
        tasks.Add(userJoined.StopConsumptionAsync(connectionId, roomId));
        async Task UnsubscribeFromFeedAsync()
        {
            var focusedRoomId = roomFocusTracker.TryGetRoomFocus(connectionId);
            if (focusedRoomId is not null)
                await messageReceived.StopConsumptionAsync(
                    connectionId, focusedRoomId.Value
                );
        }
        tasks.Add(UnsubscribeFromFeedAsync());
        await Task.WhenAll(tasks);
    }

    private async Task SubscribeToRoomAsync(Guid roomId, Guid userId)
    {
        var userConnections = connectionTracker.GetConnections(userId);
        await Task.WhenAll(
            userConnections.Select((connectionId) =>
                SubscribeToRoomAsync(roomId, connectionId)
            )
        );
    }

    private async Task SubscribeToRoomAsync(Guid roomId, string connectionId)
    {
        await Task.WhenAll(
            userLeft.StartConsumptionAsync(connectionId, roomId),
            userJoined.StartConsumptionAsync(connectionId, roomId)
        );
    }
}
