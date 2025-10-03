/*
    File: UserServiceTests.cs
    Summary: Unit test class for UserService, testing business logic operations using Moq for dependency mocking.
*/
using Moq;
using TechHiveAPI.DTOs;
using TechHiveAPI.Models;
using TechHiveAPI.Repositories;
using TechHiveAPI.Services;
using Microsoft.Extensions.Logging;

namespace TechHiveAPI.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" },
            new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Role = "Manager" }
        };
        _mockRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" };
        _mockRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_ValidUser_ReturnsCreatedUser()
    {
        // Arrange
        var userCreateDto = new UserCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Role = "Developer"
        };
        var createdUser = new User
        {
            Id = 1,
            FirstName = userCreateDto.FirstName,
            LastName = userCreateDto.LastName,
            Email = userCreateDto.Email,
            Role = userCreateDto.Role
        };
        _mockRepository.Setup(r => r.EmailExistsAsync(userCreateDto.Email, null)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(userCreateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task CreateUserAsync_DuplicateEmail_ThrowsException()
    {
        // Arrange
        var userCreateDto = new UserCreateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "existing@test.com",
            Role = "Developer"
        };
        _mockRepository.Setup(r => r.EmailExistsAsync(userCreateDto.Email, null)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUserAsync(userCreateDto));
    }

    [Fact]
    public async Task UpdateUserAsync_ExistingUser_ReturnsUpdatedUser()
    {
        // Arrange
        var userUpdateDto = new UserUpdateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Role = "Team Lead"
        };
        var existingUser = new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Role = "Developer"
        };
        var updatedUser = new User
        {
            Id = 1,
            FirstName = userUpdateDto.FirstName,
            LastName = userUpdateDto.LastName,
            Email = userUpdateDto.Email,
            Role = userUpdateDto.Role
        };
        _mockRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(existingUser);
        _mockRepository.Setup(r => r.EmailExistsAsync(userUpdateDto.Email, 1)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(updatedUser);

        // Act
        var result = await _userService.UpdateUserAsync(1, userUpdateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Team Lead", result.Role);
    }

    [Fact]
    public async Task UpdateUserAsync_NonExistingUser_ReturnsNull()
    {
        // Arrange
        var userUpdateDto = new UserUpdateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            Role = "Developer"
        };
        _mockRepository.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateUserAsync(999, userUpdateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteUserAsync_ExistingUser_ReturnsTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteUserAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteUserAsync_NonExistingUser_ReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteUserAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(999);

        // Assert
        Assert.False(result);
    }
}
