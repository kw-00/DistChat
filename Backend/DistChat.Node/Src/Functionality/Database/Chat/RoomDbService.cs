using Dapper;
using DistChat.Node.Exceptions;
using DistChat.Node.Functionality.Database.Users;
using DistChat.Node.Functionality.DTOs.Chat;
using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Functionality.Exceptions.Chat;
using DistChat.Node.Functionality.Exceptions.Users;
using DistChat.Node.Functionality.Models.Chat;
using Npgsql;

namespace DistChat.Node.Functionality.Database.Chat;

public class RoomDbService(NpgsqlDataSource dataSource)
    : IRoomDbService
{
    public async Task<GroupRoomDTO> CreateGroupRoomAsync(
        Guid id, string name, Guid creatorId, IEnumerable<Guid> memberIds
    )
    {
        var memberIdList = memberIds.ToArray();
        await using var connection = await dataSource.OpenConnectionAsync();
        try
        {
            var groupRoom = await connection.QuerySingleAsync<GroupRoomDTO>(
                $"""
                BEGIN;
            
                CREATE TEMP TABLE notFriends
                AS SELECT 1 FROM unnest(@memberIds) AS memberIds(id)
                WHERE NOT id = ANY(
                    SELECT {FriendshipTable.Columns.FriendId}
                    FROM {FriendshipTable.TableName}
                    WHERE {FriendshipTable.Columns.UserId} = @creatorId
                )
                ON COMMIT DROP;

            
                INSERT INTO {RoomTable.TableName} (
                    {RoomTable.Columns.Id},
                    {RoomTable.Columns.Name},
                    {RoomTable.Columns.Type}
                )
                SELECT
                    @id,
                    @name,
                    '{RoomTable.Type.Group}'
                WHERE NOT EXISTS (
                    SELECT 1 FROM notFriends
                )
                RETURNING *;

                INSERT INTO {MembershipTable.TableName} (
                    {MembershipTable.Columns.UserId},
                    {MembershipTable.Columns.RoomId},
                    {MembershipTable.Columns.RoleId}
                )
                SELECT friendId, @id, '{RoleTable.Role.Member}'
                FROM unnest(@memberIds) AS toBeAdded(friendId)
                UNION ALL
                SELECT @creatorId, @id, '{RoleTable.Role.Owner}';

                COMMIT;    
                """,
                new { id, creatorId, memberIds }
            );
            return groupRoom;
        }
        catch (PostgresException pgEx)
        {
            if (
                pgEx.ConstraintName
                    == MembershipTable.Constraints.FkMembershipRoomId
            )
                throw new NotFriendsException(
                    $"Some users could not be added, as they are"
                        + " not friends of user with ID of {creatorId}."
                );
            throw;
        }
    }
    public async Task<DmRoomDTO> ConnectUsersAsync(
        Guid newDmId, Guid userAId, Guid userBId
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        var orderedUserIds = new Guid[] { userAId, userBId }
            .Order()
            .ToArray();
        var userLowId = orderedUserIds[0];
        var userHighId = orderedUserIds[1];

        var defaultRole = RoleTable.Role.Member;
        try
        {
            var newDm = await connection.QuerySingleAsync<DmRoomDTO>(
                $"""
                BEGIN;
                INSERT INTO {RoomTable.TableName} (
                    {RoomTable.Columns.Id},
                    {RoomTable.Columns.Type},
                    {RoomTable.Columns.DmUserLowId},
                    {RoomTable.Columns.DmUserHighId}
                )
                SELECT 
                    @newDmId, 
                    '{RoomTable.Type.Dm}', 
                    @userLowId, 
                    @userHighId
                WHERE EXISTS (
                    SELECT 1 FROM {FriendshipTable.TableName}
                    WHERE
                        {FriendshipTable.Columns.UserId} = @userLowId
                        AND {FriendshipTable.Columns.FriendId} = @userHighId
                )
                RETURNING *;

                INSERT INTO {MembershipTable.TableName} (
                    {MembershipTable.Columns.UserId}
                    {MembershipTable.Columns.RoomId}
                    {MembershipTable.Columns.RoleId}
                )
                VALUES
                    (@userLowId, @newDmId, {defaultRole}),
                    (@userHighId, @newDmId, {defaultRole})
                ;
                COMMIT;
                """
            );
            return newDm;
        }
        catch (PostgresException pgEx)
        {
            Exception? toThrow = pgEx.ConstraintName switch
            {
                RoomTable.Constraints.UniqueDms
                    => new DuplicateDmException(userLowId, userHighId, pgEx),
                MembershipTable.Constraints.FkMembershipRoomId
                    => new NotFriendsException(userLowId, userHighId, pgEx),
                _ => null
            };
            if (toThrow is not null) throw toThrow;
            throw;
        }

    }

    public async Task<GroupRoomDTO> GetGroupRoomAsync(Guid roomId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var room = await connection.QuerySingleAsync<GroupRoomDTO>(
            $"""
            SELECT * 
            FROM {RoomTable.TableName}
            WHERE
                {RoomTable.Columns.Id} = @roomId
                AND {RoomTable.Columns.Type} = '{RoomTable.Type.Group}'
            ;
            """,
            new { roomId }
        );
        return room;
    }
    public async Task<IReadOnlyList<RoomDTO>> GetRoomsAsync(Guid userId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var rooms = await connection.QueryAsync<Room>(
            $"""
            SELECT * 
            FROM {RoomTable.TableName} r
            INNER JOIN {MembershipTable.TableName} m
                ON m.{MembershipTable.Columns.RoomId} = r.{RoomTable.Columns.Id}
            WHERE m.{MembershipTable.Columns.UserId} = @userId;
            """,
            new { userId }
        );
        return [.. rooms.Select(RoomDTO.FromRoom)];
    }
    public async Task<IReadOnlyList<PublicUserDTO>> GetUsersAsync(Guid roomId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var users = await connection.QueryAsync<User>(
            $"""
            SELECT * FROM {UserTable.TableName} u
            INNER JOIN {MembershipTable.TableName} m
                ON m.{MembershipTable.Columns.UserId} 
                    = u.{UserTable.Columns.Id}
            WHERE m.{MembershipTable.Columns.RoomId} = @roomId;
            """,
            new { roomId }
        );
        return [.. users.Select(u => new PublicUserDTO(u))];

    }
    public async Task AddUserAsync(
        Guid initiatingUserId, Guid userToBeAddedId, Guid groupRoomId
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var roleCheck = $"""
            EXISTS (
                SELECT 1 FROM {MembershipTable.TableName}
                WHERE
                    {MembershipTable.Columns.UserId} = @initiatingUserId
                    AND {MembershipTable.Columns.RoomId} = @groupRoomId
                    AND {MembershipTable.Columns.RoleId} IN (
                        '{RoleTable.Role.Owner}',
                        '{RoleTable.Role.Elder}'
                    )
            )
            """;

        var roomCheck = $"""
            EXISTS (
                SELECT 1 FROM {RoomTable.TableName}
                WHERE
                    {RoomTable.Columns.Id} = @groupRoomId
                    AND {RoomTable.Columns.Type} = {RoomTable.Type.Group}
            )
            """;

        var friendshipCheck = $"""
            EXISTS (
                SELECT 1 FROM {FriendshipTable.TableName}
                WHERE
                    {FriendshipTable.Columns.UserId} = @initatingUserId
                    AND {FriendshipTable.Columns.FriendId} = @userToBeAddedId
            )
            """;

        try
        {
            var rowCount = await connection.ExecuteAsync(
                $"""
                INSERT INTO {MembershipTable.TableName} (
                    {MembershipTable.Columns.UserId},
                    {MembershipTable.Columns.RoomId},
                    {MembershipTable.Columns.RoleId}
                )
                SELECT 
                    @userToBeAddedId,
                    @groupRoomId,
                    '{RoleTable.Role.Member}'
                WHERE 
                    {roleCheck}
                    AND {roomCheck}
                    AND {friendshipCheck}
                ;
                """,
                new { initiatingUserId, userToBeAddedId, groupRoomId }
            );
            if (rowCount == 0)
            {
                await using var checks = await connection.QueryMultipleAsync(
                    $"""
                    SELECT {roleCheck};
                    SELECT {roomCheck};
                    SELECT {friendshipCheck};
                    """
                );
                var userCanAddUsersToRoom = await checks.ReadSingleAsync<bool>();
                if (!userCanAddUsersToRoom)
                    throw new InsufficientRoomRoleException(initiatingUserId, groupRoomId);
                var roomIsGroupRoom = await checks.ReadSingleAsync<bool>();
                if (!roomIsGroupRoom)
                    throw new InvalidOperationException(
                        "Room is a DM room. Adding users to DM rooms is not allowed."
                    );
                var usersAreFriends = await checks.ReadSingleAsync<bool>();
                throw new NotFriendsException(initiatingUserId, userToBeAddedId);
            }
        }
        catch (PostgresException pgEx)
        {
            if (pgEx.ConstraintName == MembershipTable.Constraints.PrimaryKey)
                throw new InvalidOperationException(
                    "User to be added is already in room."
                );
            throw;
        }
    }
    public async Task RemoveUserAsync(
        Guid initiatingUserId, Guid userToBeRemovedId, Guid groupRoomId
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var rowCount = await connection.ExecuteAsync(
            $"""
            DELETE FROM {MembershipTable.TableName} m
            USING {RoleTable.TableName} r
            WHERE 
                {MembershipTable.Columns.UserId} = @userToBeRemovedId
                AND {MembershipTable.Columns.RoomId} = @groupRoomId
                AND r.{RoleTable.Columns.Id} = m.{MembershipTable.Columns.RoleId}
                AND r.{RoleTable.Columns.Level} < (
                    SELECT initiatorRole.{RoleTable.Columns.Level}
                    FROM {MembershipTable.TableName} initiatorMembership
                    INNER JOIN {RoleTable.TableName} initiatorRole
                        ON initiatorRole.{RoleTable.Columns.Id} 
                            = initiatorMembership.{MembershipTable.Columns.RoleId}
                    WHERE
                        initiatorMembership.{MembershipTable.Columns.UserId}
                            = @initiatingUserId
                        AND initiatorMembership.{MembershipTable.Columns.RoomId}
                            = @groupRoomId
                )
                AND EXISTS (
                    SELECT 1 FROM {RoomTable.TableName}
                    WHERE 
                        {RoomTable.Columns.Id} = @groupRoomId
                        AND {RoomTable.Columns.Type} = {RoomTable.Type.Group}
                )
            ;
            """,
            new { initiatingUserId, userToBeRemovedId, groupRoomId }
        );
        if (rowCount == 0)
            throw new DistChatException("Cannot remove user from room.");
    }

    public async Task RemoveUserAsync(Guid userId, Guid groupRoomId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var rowCount = connection.ExecuteAsync(
            $"""
            DELETE FROM {MembershipTable.TableName} m
            USING {RoomTable.TableName} r
            WHERE 
                m.{MembershipTable.Columns.UserId} = @userId
                AND m.{MembershipTable.Columns.RoomId} = @groupRoomId
                AND r.{RoomTable.Columns.Id} = @groupRoomId
                AND r.{RoomTable.Columns.Type} = '{RoomTable.Type.Group}'
            ;
            """,
            new { userId, groupRoomId }
        );
    }

    public async Task DeleteGroupRoomAsync(Guid initiatingUserId, Guid groupRoomId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var rowCount = await connection.ExecuteAsync(
            $"""
            DELETE FROM {RoomTable.TableName} r
            USING {MembershipTable.TableName} m 
            WHERE 
                r.{RoomTable.Columns.Id} = @groupRoomId
                AND m.{MembershipTable.Columns.UserId} = @initiatingUserId
                AND m.{MembershipTable.Columns.RoleId} = @groupRoomId
                AND m.{MembershipTable.Columns.RoleId} = {RoleTable.Role.Owner}
            ;
            """,
            new { initiatingUserId, groupRoomId }
        );
        if (rowCount == 0)
            throw new DistChatException("Could not delete room.");
    }

    public async Task<bool> IsUserInRoomAsync(Guid userId, Guid roomId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var isInRoom = await connection.ExecuteScalarAsync<bool>(
            $"""
            SELECT EXISTS (
                SELECT 1 FROM {MembershipTable.TableName}
                WHERE
                    {MembershipTable.Columns.UserId} = @userId
                    AND {MembershipTable.Columns.RoomId} = @roomId
            );
            """,
            new { userId, roomId }
        );
        return isInRoom;
    }

}
