using Dapper;
using DistChat.Node.Functionality.DTOs.Users;
using DistChat.Node.Functionality.Exceptions.Users;
using Npgsql;

namespace DistChat.Node.Functionality.Database.Users;

public class UserDbService(NpgsqlDataSource dataSource) : IUserDbService
{
    public async Task<User> CreateAsync(string login, string email, string passwordHash)
    {
        try
        {
            await using var connection = await dataSource.OpenConnectionAsync();
            var user = await connection.QuerySingleAsync<User>(
                $@"
                INSERT INTO {UserTable.TableName} (
                    {UserTable.Columns.Login}, 
                    {UserTable.Columns.Email}, 
                    {UserTable.Columns.PasswordHash}
                )
                VALUES (
                    @login, 
                    @email, 
                    @passwordHash
                )
                RETURNING *;
                ",
                new
                {
                    login,
                    email,
                    passwordHash
                }
            );
            return user;
        }
        catch (PostgresException pgEx)
        {
            Exception? toThrow = pgEx.ConstraintName switch
            {
                UserTable.Constraints.UniqueLogin => new LoginInUseException(login),
                UserTable.Constraints.UniqueEmail => new EmailInUseException(email),
                _ => null
            };
            if (toThrow is not null) throw toThrow;
            throw;
        }
    }

    public async Task<User?> GetAsync(Guid id)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<User>(
            $@"SELECT * FROM {UserTable.TableName} WHERE {UserTable.Columns.Id} = @id;",
            new { id }
        );
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<User>(
            $@"SELECT * FROM {UserTable.TableName} WHERE {UserTable.Columns.Login} = @login;",
            new { login }
        );
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<User>(
            $@"SELECT * FROM {UserTable.TableName} WHERE {UserTable.Columns.Email} = @email;",
            new { email }
        );
    }
    public async Task<IReadOnlyList<PublicUserDTO>> SearchAsync(string query)
    {
        throw new NotImplementedException();
    }

    public async Task ChangePasswordHashAsync(Guid userId, string passwordHash)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(
            $@"
            UPDATE {UserTable.TableName} 
            SET {UserTable.Columns.PasswordHash} = @passwordHash 
            WHERE {UserTable.Columns.Id} = @userId;
            ",
            new
            {
                userId,
                passwordHash
            }
        );
    }
}