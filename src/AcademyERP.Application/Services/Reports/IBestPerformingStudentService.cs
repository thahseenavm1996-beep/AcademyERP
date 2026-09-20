using AcademyERP.Application.DTOs.Reports;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.Services.Reports;

public interface IBestPerformingStudentService
{
    Task<BestPerformingStudentsReportResponse> GetReportAsync(
        string? courseName = null,
        Gender? gender = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null);
}
