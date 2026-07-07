namespace AcademyERP.Admin.Models;

using System.ComponentModel.DataAnnotations;

public class UpdateStudentRequest
{
    public string FullName { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public DateTime DateOfBirth { get; set; }

    public int Gender { get; set; }
    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string Country { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string? Remarks { get; set; }
}