using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechHiveAPI.Data;
using TechHiveAPI.DTOs;

namespace TechHiveAPI.Tests.Integration.Controllers;

public class UsersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private const string ValidToken = "TechHive2024SecureToken";

    public UsersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add new DbContext with unique in-memory database
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                });
            });
        });

        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ValidToken);
    }

    [Fact]
    public async Task GetAllUsers_WithValidToken_ReturnsOkAndUsers()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<List<UserReadDto>>();
        Assert.NotNull(users);
        Assert.Equal(25, users.Count); // Should have 25 seeded users
    }

    [Fact]
    public async Task GetAllUsers_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUserById_ExistingUser_ReturnsOkAndUser()
    {
        // Act
        var response = await _client.GetAsync("/api/users/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserReadDto>();
        Assert.NotNull(user);
        Assert.Equal(1, user.Id);
    }

    [Fact]
    public async Task GetUserById_NonExistingUser_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/users/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_ValidUser_ReturnsCreatedAndUser()
    {
        // Arrange
        var newUser = new UserCreateDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test.user@techhive.com",
            Role = "Developer"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", newUser);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdUser = await response.Content.ReadFromJsonAsync<UserReadDto>();
        Assert.NotNull(createdUser);
        Assert.Equal("Test", createdUser.FirstName);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var newUser = new UserCreateDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@techhive.com", // Already exists in seed data
            Role = "Developer"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", newUser);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_InvalidRole_ReturnsBadRequest()
    {
        // Arrange
        var newUser = new UserCreateDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@techhive.com",
            Role = "InvalidRole"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", newUser);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ExistingUser_ReturnsOkAndUpdatedUser()
    {
        // Arrange
        var updateDto = new UserUpdateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe.updated@techhive.com",
            Role = "Team Lead"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/1", updateDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedUser = await response.Content.ReadFromJsonAsync<UserReadDto>();
        Assert.NotNull(updatedUser);
        Assert.Equal("Team Lead", updatedUser.Role);
    }

    [Fact]
    public async Task UpdateUser_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UserUpdateDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@techhive.com",
            Role = "Developer"
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/users/999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_ExistingUser_ReturnsNoContent()
    {
        // Act
        var response = await _client.DeleteAsync("/api/users/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify user is deleted
        var getResponse = await _client.GetAsync("/api/users/1");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteUser_NonExistingUser_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/users/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
