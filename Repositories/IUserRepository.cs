using TechHiveAPI.Models;

namespace TechHiveAPI.Repositories;

/// <summary>
/// Interface for user repository operations.
/// </summary>
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<User?> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
}
