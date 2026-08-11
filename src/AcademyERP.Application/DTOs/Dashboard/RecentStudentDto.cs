namespace AcademyERP.Application.DTOs.Dashboard;

public class RecentStudentDto
{
    public Guid Id { get; set; }

    public string AdmissionNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
}