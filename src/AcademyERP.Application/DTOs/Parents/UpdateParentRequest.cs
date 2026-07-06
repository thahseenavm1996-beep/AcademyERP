using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.Parents;

public class UpdateParentRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Remarks { get; set; }
}