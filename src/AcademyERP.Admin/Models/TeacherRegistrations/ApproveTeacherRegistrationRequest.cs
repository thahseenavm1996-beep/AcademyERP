namespace AcademyERP.Admin.Models.TeacherRegistrations;

public class ApproveTeacherRegistrationRequest
{
    public string Password { get; set; } = string.Empty;
    public string? ReviewRemarks { get; set; }
}