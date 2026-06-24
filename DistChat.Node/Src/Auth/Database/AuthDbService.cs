
using Dapper;
using DistChat.Node.Auth.Models;
using DistChat.Node.Auth.Options;
using DistChat.Node.Functionality.Exceptions.Users;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DistChat.Node.Auth.Database;

public class AuthDbService(
    NpgsqlDataSource dataSource,
    IOptions<AuthOptions> options
) : IAuthDbService
{
    public async Task<RotationResult> RotateRefreshTokenAsync(
        Guid currentRefreshTokenId
    )
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var results = await connection.QueryMultipleAsync(
            $"""
            BEGIN;

            CREATE TEMP TABLE matchingToken
            AS
            SELECT * FROM {RefreshTokenTable.TableName}
            WHERE {RefreshTokenTable.Columns.Id} = @currentRefreshTokenId
            FOR UPDATE
            ON COMMIT DROP;

            DELETE FROM {RefreshTokenTable.TableName}
            WHERE
                {RefreshTokenTable.Columns.Id} = @currentRefreshTokenId
                AND {RefreshTokenTable.Columns.IsUsed}
            ;

            UPDATE {RefreshTokenTable.TableName}
            SET {RefreshTokenTable.Columns.IsUsed} = TRUE
            WHERE {RefreshTokenTable.Columns.Id} = @currentRefreshTokenId;


            SELECT * FROM matchingToken;

            INSERT INTO {RefreshTokenTable.TableName} (
                {RefreshTokenTable.Columns.UserId},
                {RefreshTokenTable.Columns.ExpiresAt}
            )
            SELECT {RefreshTokenTable.Columns.UserId}, now() + @lifetime
            FROM matchingToken
            WHERE
                NOT {RefreshTokenTable.Columns.IsUsed}
                AND {RefreshTokenTable.Columns.ExpiresAt} > now()
            RETURNING *;

            COMMIT;
            """,
            new { currentRefreshTokenId, lifetime = options.Value.RefreshTokenLifetime }
        );
        var oldToken = await results.ReadSingleOrDefaultAsync<RefreshToken>();
        var newToken = await results.ReadSingleOrDefaultAsync<RefreshToken>();
        if (oldToken is null)
            return new RotationFailure(null, RotationFailureCause.NotFound);
        if (newToken is not null)
            return new RotationSuccess(oldToken, newToken);
        if (oldToken.IsUsed)
            return new RotationFailure(oldToken, RotationFailureCause.Reuse);
        return new RotationFailure(oldToken, RotationFailureCause.Expired);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        try
        {
            var refreshToken = await connection.QuerySingleAsync<RefreshToken>(
                $"""
                INSERT INTO {RefreshTokenTable.TableName} (
                    {RefreshTokenTable.Columns.UserId},
                    {RefreshTokenTable.Columns.ExpiresAt}
                )
                VALUES (@userId, now() + @lifetime);
                """,
                new { userId, lifetime = options.Value.RefreshTokenLifetime }
            );
            return refreshToken;
        }
        catch (PostgresException pgEx)
        {
            if (pgEx.ConstraintName == RefreshTokenTable.Constraints.FkUserId)
                throw new UserNotFoundException(userId, pgEx);
            throw;
        }
    }
}

