
using Dapper;
using DistChat.Node.Auth.Models;
using DistChat.Node.Exceptions;
using DistChat.Node.Functionality.Database.Users;
using DistChat.Node.Functionality.Exceptions.Users;
using Npgsql;

namespace DistChat.Node.Auth.Database;

public class RegistrationDbService(NpgsqlDataSource dataSource)
	: IRegistrationDbService
{
    public async Task<PendingRegistration> CreatePendingRegistrationAsync(
        string login, string email, string passwordHash
	)
	{
		await using var connection = await dataSource.OpenConnectionAsync();
		try
		{
			var pendingRegistration = await connection
				.QuerySingleOrDefaultAsync<PendingRegistration>(
				$"""
				INSERT INTO {PendingRegistrationTable.TableName} (
					{PendingRegistrationTable.Columns.Login},
					{PendingRegistrationTable.Columns.Email},
					{PendingRegistrationTable.Columns.PasswordHash}
				)
				SELECT @login, @email, @passwordHash
				WHERE NOT EXISTS (
					SELECT 1 FROM {UserTable.TableName}
					WHERE
						{UserTable.Columns.Login} = @login
						OR {UserTable.Columns.Email} = @email
				)
				RETURNING *;
				""",
				new { login, email, passwordHash }
			);
			if (pendingRegistration is null)
				throw new DistChatException(
					"Could not create pending registration"
					+ $" for login of \"{login}\" and email of \"{email}\""
				);
			return pendingRegistration;

		}
		catch (PostgresException pgEx)
		{
			Exception? toThrow = pgEx.ConstraintName switch
			{
				PendingRegistrationTable.Constraints.UniqueLogin
					=> throw new LoginInUseException(login, pgEx),
				PendingRegistrationTable.Constraints.UniqueEmail
					=> throw new EmailInUseException(email, pgEx),
				_ => null
			};
			if (toThrow is not null) throw toThrow;
			throw;
		}
	}

    public async Task<User> RegisterUserAsync(Guid pendingRegistrationId)
	{
		await using var connection = await dataSource.OpenConnectionAsync();
		var user = await connection.QuerySingleOrDefaultAsync<User>(
			$"""
			BEGIN;
			
			SELECT 1 FROM {PendingRegistrationTable.TableName}
			WHERE {PendingRegistrationTable.Columns.Id} = @pendingRegistrationId
			FOR UPDATE;
			
			INSERT INTO {UserTable.TableName} (
				{UserTable.Columns.Login},
				{UserTable.Columns.Email},
				{UserTable.Columns.PasswordHash}
			)
			SELECT
				{PendingRegistrationTable.Columns.Login},
				{PendingRegistrationTable.Columns.Email},
				{PendingRegistrationTable.Columns.PasswordHash}
			FROM {PendingRegistrationTable.TableName}
			WHERE {PendingRegistrationTable.Columns.Id} = @pendingRegistrationId
			RETURNING *;

			DELETE FROM {PendingRegistrationTable.TableName}
			WHERE {PendingRegistrationTable.Columns.Id} = @pendingRegistrationId;

			COMMIT;
			""",
			new { pendingRegistrationId }
		);
		if (user is null)
			throw new DistChatException(
				$"Pending registration with ID of {pendingRegistrationId} does not exist"
			);
		return user;
	}
}
