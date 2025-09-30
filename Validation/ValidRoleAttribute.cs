using System.ComponentModel.DataAnnotations;

namespace TechHiveAPI.Validation;

/// <summary>
/// Custom validation attribute to ensure the role is one of the valid predefined roles.
/// </summary>
public class ValidRoleAttribute : ValidationAttribute
{
    private static readonly string[] ValidRoles = new[]
    {
        "Developer",
        "Manager",
        "Designer",
        "QA Engineer",
        "DevOps Engineer",
        "Product Manager",
        "Architect",
        "Team Lead"
    };

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string role)
        {
            if (ValidRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
                $"Role must be one of the following: {string.Join(", ", ValidRoles)}");
        }

        return new ValidationResult("Role is required.");
    }
}
