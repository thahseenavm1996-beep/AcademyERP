using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.Teachers;

public class ResetTeacherPasswordRequest : IValidatableObject
{
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Password != ConfirmPassword)
        {
            yield return new ValidationResult(
                "Password and Confirm Password must match.",
                new[] { nameof(ConfirmPassword) });
        }
    }
}
