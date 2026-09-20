using AcademyERP.Application.DTOs.Reports;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.Services.Reports;

public interface IBestPerformingFacultyService
{
    Task<BestPerformingFacultyReportResponse> GetReportAsync(
        string? courseName = null,
        Gender? gender = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null);
}
