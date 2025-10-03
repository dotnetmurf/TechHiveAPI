/*
    File: UserRepositoryTests.cs
    Summary: Unit test class for UserRepository, testing data access operations 
    using Moq for logging and in-memory database for test isolation.
*/
using Microsoft.EntityFrameworkCore;
using TechHiveAPI.Data;
using TechHiveAPI.Models;
using TechHiveAPI.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace TechHiveAPI.Tests.Unit.Repositories;

public class UserRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly UserRepository _repository;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        _context.Users.AddRange(
            new User { FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" },
            new User { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com", Role = "Manager" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllUsersAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
    {
        // Arrange
        var user = new User { FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByIdAsync(user.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ExistingEmail_ReturnsUser()
    {
        // Arrange
        var user = new User { FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserByEmailAsync("john@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task CreateUserAsync_AddsUser()
    {
        // Arrange
        var user = new User { FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" };

        // Act
        var result = await _repository.CreateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task UpdateUserAsync_ExistingUser_UpdatesUser()
    {
        // Arrange
        var user = new User { FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.Role = "Team Lead";

        // Act
        var result = await _repository.UpdateUserAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Team Lead", result.Role);
    }

    [Fact]
    public async Task DeleteUserAsync_ExistingUser_DeletesUser()
    {
        // Arrange
        var user = new User { FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteUserAsync(user.Id);

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Users.FindAsync(user.Id));
    }

    [Fact]
    public async Task EmailExistsAsync_ExistingEmail_ReturnsTrue()
    {
        // Arrange
        var user = new User { FirstName = "John", LastName = "Doe", Email = "john@test.com", Role = "Developer" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync("john@test.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_NonExistingEmail_ReturnsFalse()
    {
        // Act
        var result = await _repository.EmailExistsAsync("nonexisting@test.com");

        // Assert
        Assert.False(result);
    }
}
