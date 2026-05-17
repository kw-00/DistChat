using DistChat.Node.Functionality.DTOs.Users;

namespace DistChat.Node.Functionality.Database.Users;

public interface IUserDbService
{
    Task<User> CreateAsync(string name, string email, string passwordHash);

    Task<User?> GetAsync(Guid id);

    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetByEmailAsync(string email);
    Task<IReadOnlyList<PublicUserDTO>> SearchAsync(string query);

    Task ChangePasswordHashAsync(Guid userId, string passwordHash);
}