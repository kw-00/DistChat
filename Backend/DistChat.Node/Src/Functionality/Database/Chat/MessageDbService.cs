using Dapper;
using DistChat.Node.Functionality.Exceptions.Users;
using Npgsql;

namespace DistChat.Node.Functionality.Database.Chat;

public class MessageDbService(NpgsqlDataSource dataSource) : IMessageDbService
{
    public async Task<Message> CreateAsync(
        Guid userId, Guid roomId, string content
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        try
        {            
            var message = await connection.QuerySingleOrDefaultAsync<Message>(
                $"""
                INSERT INTO {MessageTable.TableName} (
                    {MessageTable.Columns.UserId},
                    {MessageTable.Columns.RoomId},
                    {MessageTable.Columns.Content}
                )
                SELECT @userId, @roomId, @content
                WHERE EXISTS (
                    SELECT 1 FROM {MembershipTable.TableName}
                    WHERE 
                        {MembershipTable.Columns.UserId} = @userId
                        AND {MembershipTable.Columns.RoomId} = @roomId
                )
                RETURNING *;
                """,
                new { userId, roomId, content }
            );
            if (message is null)
                throw new Exception("User is not in room.");
            return message;
        }
        catch (PostgresException pgEx)
        {
            Exception? toThrow = pgEx.ConstraintName switch
            {
                MessageTable.Constraints.FkUserId
                    => new UserNotFoundException(userId, pgEx),
                MessageTable.Constraints.FkRoomId
                    => new RoomNotFoundException(roomId, pgEx),
                _ => null
            };
            if (toThrow is not null) throw toThrow;
            throw;
        }
    }

    public async Task<IReadOnlyList<Message>> GetMessagesAsync(
        Guid inquiringUserId,
        Guid roomId, 
        int limit, 
        Guid? before = null, 
        Guid? after = null, 
        bool newestFirst = false
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var orderBy = newestFirst ? "ASC" : "DESC";
        var messages = await connection.QueryAsync(
            $"""
            SELECT *
            FROM {MessageTable.TableName}
            WHERE 
                {MessageTable.Columns.RoomId} = @roomId
                AND (@before IS NULL OR {MessageTable.Columns.Id} < @before)
                AND (@after IS NULL OR {MessageTable.Columns.Id} > @after)
                AND EXISTS (
                    SELECT 1 FROM {MembershipTable.TableName}
                    WHERE 
                        {MembershipTable.Columns.UserId} = @inquiringUserId
                        AND {MembershipTable.Columns.RoomId} = @roomId
                )
            ORDER BY
                {MessageTable.Columns.Id} {orderBy}
            LIMIT {limit};
            """,
            new { 
                inquiringUserId, 
                roomId, 
                before, 
                after, 
                newestFirst, 
                limit 
            }
        );
        return [.. messages];
    }
}