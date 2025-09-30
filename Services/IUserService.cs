using TechHiveAPI.DTOs;

namespace TechHiveAPI.Services;

/// <summary>
/// Interface for user service operations.
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
    Task<UserReadDto?> GetUserByIdAsync(int id);
    Task<UserReadDto> CreateUserAsync(UserCreateDto userCreateDto);
    Task<UserReadDto?> UpdateUserAsync(int id, UserUpdateDto userUpdateDto);
    Task<bool> DeleteUserAsync(int id);
}
