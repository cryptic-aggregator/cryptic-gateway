using GatewayService.Database.Tables;

namespace GatewayService.Interfaces.Repositories;

public interface IUserRepository
{
    Task<int> CreateAsync(UserTable user);

    Task<UserTable> GetByIdAsync(int id);

    Task<UserTable> GetByEmailAsync(string email);
}