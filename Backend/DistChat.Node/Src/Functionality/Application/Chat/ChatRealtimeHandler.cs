using System.Text.Json;
using DistChat.Node.Infrastructure.RealtimeHub;

namespace DistChat.Node.Functionality.Application.Chat;

public class ChatRealtimeHandler
{
    public ChatRealtimeHandler(
        CommandDispatcher dispatcher,
        IChatOperations operations,
        UserConnectionTracker connectionTracker,
        JsonSerializerOptions jsonSerializerOptions
    )
    {

        var group = new CommandGroup();

        group.RegisterCommand("createRoom", async (invocation) =>
        {
            if (invocation.Args.Length != 2)
                throw new InvalidCommandInvocationException(
                    $"Expected two arguments, got {invocation.Args.Length}."
                );
            var memberIdsJson = JsonSerializer.Serialize(invocation.Args[0]);
            var memberIds = JsonSerializer.Deserialize<IEnumerable<Guid>>(
                memberIdsJson, jsonSerializerOptions
            ) ?? throw new InvalidCommandInvocationException(
                "Deserialized memberIds argument is null."
            );
            var name = invocation.Args[1].ToString() ?? "";

            await operations.CreateRoomAsync(
                invocation.ConnectionId, memberIds, name
            );
        });

        group.RegisterCommand("getRooms", async (invocation) =>
        {
            await operations.GetRoomsAsync(invocation.ConnectionId);
        });

        group.RegisterCommand("addUser", async (invocation) => 
        { 
            var roomId = ParseRoomId(invocation.Args[0]);
            var userToAddIdValid = Guid.TryParse(
                invocation.Args[1].ToString(), out var userToAddId
            );
            if (!userToAddIdValid)
                throw new InvalidCommandInvocationException(
                    "UserToAddId is not a valid Guid."
                );

            await operations.AddUserAsync(
                invocation.ConnectionId, roomId, userToAddId
            );
        });

        group.RegisterCommand("removeUser", async (invocation) => 
        { 
            var roomId = ParseRoomId(invocation.Args[0]);
            var userToRemoveIdValid = Guid.TryParse(
                invocation.Args[1].ToString(), out var userToRemoveId
            );
            if (!userToRemoveIdValid)
                throw new InvalidCommandInvocationException(
                    "UserToRemoveId is not a valid Guid."
                );
                
            await operations.RemoveUserAsync(
                invocation.ConnectionId, roomId, userToRemoveId
            );
        });

        group.RegisterCommand("deleteRoom", async (invocation) => 
        { 
            var roomId = ParseRoomId(invocation.Args[0]);
            await operations.DeleteRoomAsync(invocation.ConnectionId, roomId);
        });

        group.RegisterCommand("leaveRoom", async (invocation) => 
        { 
            var roomId = ParseRoomId(invocation.Args[0]);
            await operations.LeaveRoomAsync(invocation.ConnectionId, roomId);
        });

        group.RegisterCommand("sendMessage", async (invocation) =>
        {
            var roomId = ParseRoomId(invocation.Args[0]);
            var content = invocation.Args[1]?.ToString()
                ?? throw new InvalidCommandInvocationException(
                    "Content is null or absent."
                );

            await operations.SendMessageAsync(
                invocation.ConnectionId,
                roomId,
                content
            );
        });

        group.RegisterCommand("focusRoom", async (invocation) =>
        {
            var roomIdArg = invocation.Args[0]?.ToString();

            Guid? roomId =
                roomIdArg is null 
                ? null 
                : Guid.TryParse(roomIdArg, out var parsed) 
                    ? parsed 
                    : throw new InvalidCommandInvocationException(
                        "Room ID is not a valid Guid."
                    );

            return await operations.FocusRoomAsync(invocation.ConnectionId, roomId);
        });

        group.RegisterCommand("goOlder", async (invocation) =>
        {
            var oldestmessageIdValid = Guid.TryParse(
                invocation.Args[0]?.ToString() ?? "",
                out var oldestMessageId
            );

            if (!oldestmessageIdValid)
                throw new InvalidCommandInvocationException(
                    "Oldest message ID is not a valid Guid."
                );

            return await operations.GoOlderAsync(
                invocation.ConnectionId,
                oldestMessageId
            );
        });

        group.RegisterCommand("goNewer", async (invocation) =>
        {
            var newestMessageIdValid = Guid.TryParse(
                invocation.Args[0]?.ToString() ?? "",
                out var newestMessageId
            );

            if (!newestMessageIdValid)
                throw new InvalidCommandInvocationException(
                    "Newest message ID is not a valid Guid."
                );

            return await operations.GoNewerAsync(
                invocation.ConnectionId,
                newestMessageId
            );
        });

        connectionTracker.UserConnected += async (conn) =>
        {
            await operations.HandleConnectedAsync(
                conn.ConnectionId, conn.UserId
            );
        };

        connectionTracker.UserDisconnected += async (conn) =>
        {
            await operations.HandleDisconnectedAsync(
                conn.ConnectionId, conn.UserId
            );
        };
        dispatcher.RegisterGroup("chat", group);
    }

    private static Guid ParseRoomId(object obj)
    {
        var roomIdIsValid = Guid.TryParse(
            obj.ToString(),
            out var roomId
        );
        if (!roomIdIsValid)
            throw new InvalidCommandInvocationException(
                "Room ID is not a valid Guid."
            );
        return roomId;
    }
}