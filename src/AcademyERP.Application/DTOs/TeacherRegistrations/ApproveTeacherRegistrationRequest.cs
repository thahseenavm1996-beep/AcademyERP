using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Application.DTOs.TeacherRegistrations;

public class ApproveTeacherRegistrationRequest
{
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    public string? ReviewRemarks { get; set; }
}
