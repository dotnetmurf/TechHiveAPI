/*
    File: User.cs
    Summary: Entity Framework model representing a user entity with properties for ID, name, email, and role.
*/
namespace TechHiveAPI.Models;

/// <summary>
/// Represents a user entity in the system.
/// </summary>
public class User
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}
