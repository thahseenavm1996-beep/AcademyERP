using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.Interfaces;

public interface IClassReportService
{
    Task<ClassReportResponse> CreateAsync(CreateClassReportRequest request);

    Task<ClassReportResponse?> GetByIdAsync(Guid id);

    Task<PagedResponse<ClassReportResponse>> GetAllAsync(ClassReportQueryRequest request);

    Task<ClassReportResponse?> UpdateAsync(Guid id, UpdateClassReportRequest request);

    Task<bool> DeleteAsync(Guid id);

    Task<ClassReportResponse?> GetByScheduledClassIdAsync(
    Guid scheduledClassId);

    Task<List<ClassReportResponse>> GetForExportAsync(
    ClassReportQueryRequest request);

    Task<List<TeacherReportMonitoringResponse>> GetTeacherReportMonitoringAsync(
    DateTime? fromDate,
    DateTime? toDate,
    Guid? teacherId = null,
    string? courseName = null,
    Gender? gender = null,
    bool? reportSubmitted = null);
}