using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.Parents;

public class CreateParentRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Password != ConfirmPassword)
        {
            yield return new ValidationResult(
                "Password and Confirm Password must match.",
                new[]
                {
                nameof(ConfirmPassword)
                });
        }
    }

    public string? Remarks { get; set; }
}