using AcademyERP.Application.DTOs.StudentPerformance;

namespace AcademyERP.Application.Services;

public interface IStudentPerformanceService
{
    Task<StudentPerformanceResponse>
        GetAsync(Guid studentId);
}