using TechHiveAPI.DTOs;
using TechHiveAPI.Models;
using TechHiveAPI.Repositories;

namespace TechHiveAPI.Services;

/// <summary>
/// Service implementation for user business logic.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
    {
        try
        {
            var users = await _userRepository.GetAllUsersAsync();
            return users.Select(MapToReadDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving all users");
            throw;
        }
    }

    public async Task<UserReadDto?> GetUserByIdAsync(int id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user == null ? null : MapToReadDto(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<UserReadDto> CreateUserAsync(UserCreateDto userCreateDto)
    {
        try
        {
            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(userCreateDto.Email))
            {
                throw new InvalidOperationException($"A user with email '{userCreateDto.Email}' already exists.");
            }

            var user = new User
            {
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                Email = userCreateDto.Email,
                Role = userCreateDto.Role
            };

            var createdUser = await _userRepository.CreateUserAsync(user);
            _logger.LogInformation("User created successfully with ID {UserId}", createdUser.Id);
            return MapToReadDto(createdUser);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user");
            throw;
        }
    }

    public async Task<UserReadDto?> UpdateUserAsync(int id, UserUpdateDto userUpdateDto)
    {
        try
        {
            // Check if user exists
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            // Check if email already exists for another user
            if (await _userRepository.EmailExistsAsync(userUpdateDto.Email, id))
            {
                throw new InvalidOperationException($"A user with email '{userUpdateDto.Email}' already exists.");
            }

            var user = new User
            {
                Id = id,
                FirstName = userUpdateDto.FirstName,
                LastName = userUpdateDto.LastName,
                Email = userUpdateDto.Email,
                Role = userUpdateDto.Role
            };

            var updatedUser = await _userRepository.UpdateUserAsync(user);
            if (updatedUser == null)
            {
                return null;
            }

            _logger.LogInformation("User updated successfully with ID {UserId}", id);
            return MapToReadDto(updatedUser);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var result = await _userRepository.DeleteUserAsync(id);
            if (result)
            {
                _logger.LogInformation("User deleted successfully with ID {UserId}", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID {UserId}", id);
            throw;
        }
    }

    private static UserReadDto MapToReadDto(User user)
    {
        return new UserReadDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role
        };
    }
}
