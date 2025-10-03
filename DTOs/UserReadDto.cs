/*
    File: UserReadDto.cs
    Summary: DTO for reading user information. 
    Used for output responses, exposing non-sensitive user fields.
*/
namespace TechHiveAPI.DTOs;

/// <summary>
/// Data transfer object for reading user information.
/// </summary>
public class UserReadDto
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}
