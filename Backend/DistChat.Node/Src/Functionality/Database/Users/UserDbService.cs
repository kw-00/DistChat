using System.Data;
using Dapper;
using DistChat.Node.Functionality.DTOs.Users;
using Npgsql;

namespace DistChat.Node.Functionality.Database.Users;

public class UserDbService(IDbConnection connection) : IUserDbService
{
    public async Task<User> CreateAsync(string login, string email, string passwordHash)
    {
        try
        {
            var user =await connection.QuerySingleAsync<User>(
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
            Exception toThrow = pgEx.ConstraintName switch
            {
                UserTable.Constraints.UniqueLogin => new LoginInUseException(login),
                UserTable.Constraints.UniqueEmail => new EmailInUseException(email),
                _ => pgEx
            };
            throw toThrow;
        }
    }

    public async Task<User?> GetAsync(Guid id)
    {
        return await connection.QuerySingleOrDefaultAsync<User>(
            $@"SELECT * FROM {UserTable.TableName} WHERE {UserTable.Columns.Id} = @id;",
            new { id }
        );
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await connection.QuerySingleOrDefaultAsync<User>(
            $@"SELECT * FROM {UserTable.TableName} WHERE {UserTable.Columns.Login} = @login;",
            new { login }
        );
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
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