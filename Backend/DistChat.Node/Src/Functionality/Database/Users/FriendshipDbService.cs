using Dapper;
using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Functionality.Exceptions.Users;
using DistChat.Node.Infrastructure.Concurrency;
using Npgsql;

namespace DistChat.Node.Functionality.Database.Users;

public class FriendshipDbService(
    NpgsqlDataSource dataSource
) : IFriendshipDbService
{

    public async Task RequestFriendshipAsync(Guid requestingUserId, Guid targetUserId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        try
        {
            var rowCount = await connection.ExecuteAsync(
                $"""
                INSERT INTO {FriendRequestTable.TableName} (
                    {FriendRequestTable.Columns.RequestingUserId}, 
                    {FriendRequestTable.Columns.TargetUserId}
                ) 
                SELECT @requestingUserId, @targetUserId
                WHERE NOT EXISTS (
                    SELECT 1 FROM {FriendshipTable.TableName} 
                    WHERE 
                        {FriendshipTable.Columns.UserId} = @requestingUserId
                        AND {FriendshipTable.Columns.FriendId} = @targetUserId
                )
                """,
                new
                {
                    requestingUserId,
                    targetUserId
                }
            );
            if (rowCount == 0) 
                throw new AlreadyFriendsException(requestingUserId, targetUserId);
        }
        catch (PostgresException pgEx)
        {
            Exception? toThrow = pgEx.ConstraintName switch
            {
                FriendRequestTable.Constraints.PrimaryKey => 
                    new RedundantFriendRequestException(
                        requestingUserId, targetUserId, pgEx
                    ),
                FriendRequestTable.Constraints.NoMutualRequests => 
                    new RedundantFriendRequestException(
                        requestingUserId, targetUserId, pgEx
                    ),
                UserTable.Constraints.PrimaryKey => 
                    new UserNotFoundException(targetUserId, pgEx),
                _ => null
            };
            if (toThrow is not null) throw toThrow;
            throw;
        }
    }

    public async Task AcceptFriendshipAsync(
        Guid acceptingUserId, Guid requestingUserId
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        try
        {
            var insertedCount = await connection.ExecuteAsync(
                $"""
                WITH
                    request AS (
                        DELETE FROM {FriendRequestTable.TableName} 
                        WHERE 
                            {FriendRequestTable.Columns.RequestingUserId} 
                                = @requestingUserId
                            AND {FriendRequestTable.Columns.TargetUserId} 
                                = @acceptingUserId
                        RETURNING 1
                    )
                INSERT INTO {FriendshipTable.TableName} (
                    {FriendshipTable.Columns.UserId}, 
                    {FriendshipTable.Columns.FriendId}
                ) 
                SELECT v.userId, v.friendId
                FROM (
                    VALUES 
                        (@requestingUserId, @acceptingUserId), 
                        (@acceptingUserId, @requestingUserId)
                ) AS v(userId, friendId)
                WHERE
                    EXISTS (SELECT 1 FROM request)
                ;
                """,
                new
                {
                    requestingUserId,
                    acceptingUserId
                }
            );
            if (insertedCount == 0)
                throw new FriendshipNotRequestedException(
                    requestingUserId, acceptingUserId
                );
        }
        catch (PostgresException pgEx)
        {
            Exception? toThrow = pgEx.ConstraintName switch
            {
                FriendshipTable.Constraints.PrimaryKey
                    => new AlreadyFriendsException(
                        acceptingUserId, requestingUserId, pgEx
                    ),
                FriendshipTable.Constraints.FkFriendId
                    => new UserNotFoundException(
                        $"User with ID of {requestingUserId}"
                        + " or {acceptingUserId} not found.",
                        pgEx
                    ),
                FriendshipTable.Constraints.FkUserId
                    => new UserNotFoundException(
                        $"User with ID of {requestingUserId}" 
                        + " or {acceptingUserId} not found.",
                        pgEx
                    ),
                _ => null
            };
            if (toThrow is not null) throw toThrow;
            throw;
        }
    }

    public async Task<IReadOnlyList<PublicUserDTO>> GetFriendsAsync(Guid userId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var friends = await connection.QueryAsync<PublicUserDTO>(
            $"""
            SELECT u.{UserTable.Columns.Id}, u.{UserTable.Columns.Login}
            FROM {UserTable.TableName} u
            INNER JOIN {FriendshipTable.TableName} f 
                ON f.{FriendshipTable.Columns.FriendId} = u.{UserTable.Columns.Id}
            WHERE f.{FriendshipTable.Columns.UserId} = @userId;
            """,
            new { userId }
        );
        return [.. friends];
    }

    public async Task<IReadOnlyList<PublicUserDTO>> GetIncomingFriendRequestsAsync(
        Guid userId
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var requests = await connection.QueryAsync<PublicUserDTO>(
            $"""
            SELECT u.{UserTable.Columns.Id}, u.{UserTable.Columns.Login}
            FROM {UserTable.TableName} u
            INNER JOIN {FriendRequestTable.TableName} f 
                ON f.{FriendRequestTable.Columns.RequestingUserId} 
                    = u.{UserTable.Columns.Id}
            WHERE f.{FriendRequestTable.Columns.TargetUserId} = @userId;
            """,
            new { userId }
        );
        return [.. requests];
    }
    
    public async Task DeclineFriendRequestAsync(
        Guid decliningUserId, Guid requestingUserId
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(
            $"""
            DELETE FROM {FriendRequestTable.TableName} 
            WHERE 
                {FriendRequestTable.Columns.RequestingUserId} = @requestingUserId
                AND {FriendRequestTable.Columns.TargetUserId} = @decliningUserId
            ;
            """,
            new
            {
                requestingUserId,
                decliningUserId
            }
        );
    }

    public async Task UnfriendAsync(Guid initiatingUserId, Guid friendUserId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(
            $"""
            DELETE FROM {FriendshipTable.TableName} 
            WHERE 
                {FriendshipTable.Columns.UserId} = @initiatingUserId
                AND {FriendshipTable.Columns.FriendId} = @friendUserId
                OR
                {FriendshipTable.Columns.UserId} = @friendUserId
                AND {FriendshipTable.Columns.FriendId} = @initiatingUserId
            ;
            """,
            new
            {
                initiatingUserId,
                friendUserId
            }
        );
    }
}

