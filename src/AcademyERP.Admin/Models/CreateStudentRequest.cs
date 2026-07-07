namespace AcademyERP.Admin.Models;

using System.ComponentModel.DataAnnotations;

public class CreateStudentRequest : IValidatableObject
{
    /* [Required]
     public string AdmissionNumber { get; set; } = string.Empty;*/

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime? AdmissionDate { get; set; } = DateTime.Today;

    [Required]
    public DateTime? DateOfBirth { get; set; } = DateTime.Today;

    [Required]
    public int Gender { get; set; }

    [Required]
    public string MobileNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    [Required]
    public string TimeZone { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string? Remarks { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth.HasValue)
        {
            var minimumBirthDate = DateTime.Today.AddYears(-4);

            if (DateOfBirth.Value > minimumBirthDate)
            {
                yield return new ValidationResult(
                    "Student must be at least 4 years old.",
                    new[] { nameof(DateOfBirth) });
            }
        }
    }
}