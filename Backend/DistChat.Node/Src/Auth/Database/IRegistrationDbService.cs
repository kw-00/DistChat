using DistChat.Node.Auth.Models;
using DistChat.Node.Functionality.Database.Users;
using DistChat.Node.Functionality.DTOs.Users;

namespace DistChat.Node.Auth.Database;

public interface IRegistrationDbService
{
    public Task<PendingRegistration> CreatePendingRegistrationAsync(
        string login, string email, string password
    );

    public Task<User> RegisterUserAsync(Guid pendingRegistrationId);
}