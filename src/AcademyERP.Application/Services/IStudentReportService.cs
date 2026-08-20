using AcademyERP.Application.DTOs.StudentReports;

namespace AcademyERP.Application.Services;

public interface IStudentReportService
{
    Task<StudentReportResponse?> GetStudentReportAsync(Guid studentId);
}