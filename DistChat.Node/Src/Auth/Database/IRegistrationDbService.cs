using DistChat.Node.Auth.Models;
using DistChat.Node.Functionality.Database.Users;

namespace DistChat.Node.Auth.Database;

public interface IRegistrationDbService
{
    Task<PendingRegistration> CreatePendingRegistrationAsync(
        string login, string email, string passwordHash
    );

    Task<User> RegisterUserAsync(Guid pendingRegistrationId);
}
