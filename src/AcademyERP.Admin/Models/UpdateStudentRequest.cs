namespace AcademyERP.Admin.Models;

public class UpdateStudentRequest
{
    public string FullName { get; set; } = string.Empty;

    public DateTime AdmissionDate { get; set; }

    public DateTime DateOfBirth { get; set; }

    public int Gender { get; set; }

    public string Country { get; set; } = string.Empty;

    public string TimeZone { get; set; } = string.Empty;

    public string? Remarks { get; set; }
}