namespace AcademyERP.Admin.Models;

using System.ComponentModel.DataAnnotations;

public class UpdateTeacherRequest : IValidatableObject
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime? JoiningDate { get; set; }

    [Required]
    public DateTime? DateOfBirth { get; set; }

    [Required]
    public int Gender { get; set; }

    [Required]
    public string MobileNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Qualification { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    public string TimeZone { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? Remarks { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        yield break;
    }
}