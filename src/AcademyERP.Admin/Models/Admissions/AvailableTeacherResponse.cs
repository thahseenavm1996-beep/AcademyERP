namespace AcademyERP.Admin.Models.Admissions;

public class AvailableTeacherResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? MobileNumber { get; set; }
}
