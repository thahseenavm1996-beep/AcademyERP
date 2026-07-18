using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.ClassReports;

namespace AcademyERP.Application.Interfaces;

public interface IClassReportService
{
    Task<ClassReportResponse> CreateAsync(CreateClassReportRequest request);

    Task<ClassReportResponse?> GetByIdAsync(Guid id);

    Task<PagedResponse<ClassReportResponse>> GetAllAsync(ClassReportQueryRequest request);

    Task<ClassReportResponse?> UpdateAsync(Guid id, UpdateClassReportRequest request);

    Task<bool> DeleteAsync(Guid id);
}