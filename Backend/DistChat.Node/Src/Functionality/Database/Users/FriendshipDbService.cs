using System.Data;
using System.Data.Common;
using Dapper;
using DistChat.Node.Exceptions;
using DistChat.Node.Functionality.DTOs.Users;
using Npgsql;

namespace DistChat.Node.Functionality.Database.Users;

public class FriendshipDbService(IDbConnection connection) : IFriendshipDbService
{
    public async Task RequestFriendshipAsync(Guid requestingUserId, Guid targetUserId)
    {
        try
        {
            var rowCount = await connection.ExecuteAsync(
                $@"
                WITH
                    friendship AS (
                        SELECT 1 FROM {FriendshipTable.TableName} 
                        WHERE 
                            {FriendshipTable.Columns.UserId} = @requestingUserId
                            AND {FriendshipTable.Columns.FriendId} = @targetUserId
                    )
                INSERT INTO {FriendRequestTable.TableName} (
                    {FriendRequestTable.Columns.RequestingUserId}, 
                    {FriendRequestTable.Columns.TargetUserId}
                ) 
                SELECT @requestingUserId, @targetUserId
                WHERE NOT EXISTS (SELECT 1 FROM friendship)
                ",
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
            Exception toThrow = pgEx.ConstraintName switch
            {
                FriendRequestTable.Constraints.PrimaryKey => 
                    new RedundantFriendRequestException(requestingUserId, targetUserId),
                FriendRequestTable.Constraints.NoMutualRequests => 
                    new RedundantFriendRequestException(requestingUserId, targetUserId),
                UserTable.Constraints.PrimaryKey => 
                    new UserNotFoundException(targetUserId),
                _ => pgEx
            };
            throw toThrow;
        }
    }

    public async Task AcceptFriendshipAsync(
        Guid acceptingUserId, Guid requestingUserId
    )
    {
        try
        {
            var insertedCount = await connection.ExecuteAsync(
                $@"
                WITH
                    request AS (
                        DELETE FROM {FriendRequestTable.TableName} 
                        WHERE 
                            {FriendRequestTable.Columns.RequestingUserId} = @requestingUserId
                            AND {FriendRequestTable.Columns.TargetUserId} = @acceptingUserId
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
                ",
                new
                {
                    requestingUserId,
                    acceptingUserId
                }
            );
            if (insertedCount == 0)
                throw new FriendshipNotRequestedException(requestingUserId, acceptingUserId);
        }
        catch (PostgresException pgEx)
        {
            Exception toThrow = pgEx.ConstraintName switch
            {
                FriendshipTable.Constraints.PrimaryKey
                    => new AlreadyFriendsException(acceptingUserId, requestingUserId),
                FriendshipTable.Constraints.FkFriendId
                    => new UserNotFoundException(
                        $"User with ID of {requestingUserId} or {acceptingUserId} not found."
                    ),
                FriendshipTable.Constraints.FkUserId
                    => new UserNotFoundException(
                        $"User with ID of {requestingUserId} or {acceptingUserId} not found."
                    ),
                _ => pgEx
            };
            throw toThrow;
        }
    }

    public async Task<IReadOnlyList<PublicUserDTO>> GetFriendsAsync(Guid userId)
    {
        var friends = await connection.QueryAsync<PublicUserDTO>(
            $@"
            SELECT u.{UserTable.Columns.Id}, u.{UserTable.Columns.Login}
            FROM {UserTable.TableName} u
            INNER JOIN {FriendshipTable.TableName} f 
                ON f.{FriendshipTable.Columns.FriendId} = u.{UserTable.Columns.Id}
            WHERE f.{FriendshipTable.Columns.UserId} = @userId;
            ",
            new { userId }
        );
        return [.. friends];
    }

    public async Task<IReadOnlyList<PublicUserDTO>> GetIncomingFriendRequestsAsync(Guid userId)
    {
        var requests = await connection.QueryAsync<PublicUserDTO>(
            $@"
            SELECT u.{UserTable.Columns.Id}, u.{UserTable.Columns.Login}
            FROM {UserTable.TableName} u
            INNER JOIN {FriendRequestTable.TableName} f 
                ON f.{FriendRequestTable.Columns.RequestingUserId} = u.{UserTable.Columns.Id}
            WHERE f.{FriendRequestTable.Columns.TargetUserId} = @userId;
            ",
            new { userId }
        );
        return [.. requests];
    }
    
    public async Task DeclineFriendshipAsync(Guid decliningUserId, Guid requestingUserId)
    {
        await connection.ExecuteAsync(
            $@"
            DELETE FROM {FriendRequestTable.TableName} 
            WHERE 
                {FriendRequestTable.Columns.RequestingUserId} = @requestingUserId
                AND {FriendRequestTable.Columns.TargetUserId} = @decliningUserId
            ;
            ",
            new
            {
                requestingUserId,
                decliningUserId
            }
        );
    }

    public async Task UnfriendAsync(Guid initiatingUserId, Guid friendUserId)
    {
        await connection.ExecuteAsync(
            $@"
            DELETE FROM {FriendshipTable.TableName} 
            WHERE 
                {FriendshipTable.Columns.UserId} = @initiatingUserId
                AND {FriendshipTable.Columns.FriendId} = @friendUserId
                OR
                {FriendshipTable.Columns.UserId} = @friendUserId
                AND {FriendshipTable.Columns.FriendId} = @initiatingUserId
            ;
            ",
            new
            {
                initiatingUserId,
                friendUserId
            }
        );
    }
}