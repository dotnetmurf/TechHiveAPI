/*
    File: UsersController.cs
    Summary: Handles CRUD operations for user management. 
    Implements endpoints for creating, reading, updating, and deleting users with validation, error handling, and logging.
*/
using Microsoft.AspNetCore.Mvc;
using TechHiveAPI.DTOs;
using TechHiveAPI.Services;

namespace TechHiveAPI.Controllers;

/// <summary>
/// Controller for managing user operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    /// <response code="200">Returns the list of users.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserReadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAllUsers()
    {
        _logger.LogInformation("Getting all users");
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Gets a specific user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The requested user.</returns>
    /// <response code="200">Returns the requested user.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserReadDto>> GetUserById(int id)
    {
        _logger.LogInformation("Getting user with ID {UserId}", id);
        var user = await _userService.GetUserByIdAsync(id);
        
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="userCreateDto">The user data to create.</param>
    /// <returns>The newly created user.</returns>
    /// <response code="201">Returns the newly created user.</response>
    /// <response code="400">If the user data is invalid or email already exists.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserReadDto>> CreateUser([FromBody] UserCreateDto userCreateDto)
    {
        _logger.LogInformation("Creating new user with email {Email}", userCreateDto.Email);

        try
        {
            var createdUser = await _userService.CreateUserAsync(userCreateDto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="userUpdateDto">The updated user data.</param>
    /// <returns>The updated user.</returns>
    /// <response code="200">Returns the updated user.</response>
    /// <response code="400">If the user data is invalid or email already exists.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserReadDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserReadDto>> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
    {
        _logger.LogInformation("Updating user with ID {UserId}", id);

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, userUpdateDto);
            
            if (updatedUser == null)
            {
                return NotFound();
            }

            return Ok(updatedUser);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a user.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the user was successfully deleted.</response>
    /// <response code="404">If the user is not found.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        _logger.LogInformation("Deleting user with ID {UserId}", id);
        var result = await _userService.DeleteUserAsync(id);
        
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
