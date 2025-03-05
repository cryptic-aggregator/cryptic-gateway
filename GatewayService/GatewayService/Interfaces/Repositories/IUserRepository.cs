using GatewayService.Database.Tables;

namespace GatewayService.Interfaces.Repositories;

public interface IUserRepository
{
    Task<int> CreateAsync(UserTable user);

    Task<UserTable> GetByIdAsync(int id);

    Task<UserTable> GetByEmailAsync(string email);

    Task DeleteUserAsync(int id);

    Task UpdateUserProfileAsync(int id, string name, string email);

    Task UpdateUserPasswordAsync(int id, string passwordHash);
}