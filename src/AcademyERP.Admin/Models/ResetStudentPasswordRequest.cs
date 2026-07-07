using System.ComponentModel.DataAnnotations;

namespace AcademyERP.Admin.Models;

public class ResetStudentPasswordRequest
{
    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
}