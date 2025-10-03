/*
    File: UserUpdateDto.cs
    Summary: DTO for updating an existing user. 
    Includes validation for all fields and enforces required constraints and valid role.
*/
using System.ComponentModel.DataAnnotations;
using TechHiveAPI.Validation;

namespace TechHiveAPI.DTOs;

/// <summary>
/// Data transfer object for updating an existing user.
/// </summary>
public class UserUpdateDto
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(25, ErrorMessage = "First name cannot exceed 25 characters.")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(25, ErrorMessage = "Last name cannot exceed 25 characters.")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    [StringLength(25, ErrorMessage = "Role cannot exceed 25 characters.")]
    [ValidRole]
    public required string Role { get; set; }
}
