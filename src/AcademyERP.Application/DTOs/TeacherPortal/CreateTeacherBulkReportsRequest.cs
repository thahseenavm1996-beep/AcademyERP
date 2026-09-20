using AcademyERP.Application.DTOs.ClassReports;

namespace AcademyERP.Application.DTOs.TeacherPortal;

public class CreateTeacherBulkReportsRequest
{
    public List<CreateClassReportRequest> Reports { get; set; } = new();
}
