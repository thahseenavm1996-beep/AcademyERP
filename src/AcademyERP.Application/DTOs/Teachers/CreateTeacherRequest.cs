namespace AcademyERP.Application.DTOs.Teachers;

using System.ComponentModel.DataAnnotations;
using AcademyERP.Domain.Enums;

public class CreateTeacherRequest : IValidatableObject
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime? JoiningDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime? DateOfBirth { get; set; } = DateTime.Today;

    [Required]
    public Gender Gender { get; set; }

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
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string? Address { get; set; }

    public string? Remarks { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (DateOfBirth.HasValue &&
            DateOfBirth.Value > DateTime.Today.AddYears(-18))
        {
            yield return new ValidationResult(
                "Teacher must be at least 18 years old.",
                new[] { nameof(DateOfBirth) });
        }

        if (JoiningDate.HasValue &&
            JoiningDate.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult(
                "Joining date cannot be in the future.",
                new[] { nameof(JoiningDate) });
        }
    }
}