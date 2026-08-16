using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Attendance;

namespace AcademyERP.Application.Services;

public interface IAttendanceService
{
    Task<PagedResponse<AttendanceResponse>> GetHistoryAsync(AttendanceQueryRequest request);
    Task<AttendanceResponse?> GetByIdAsync(Guid id);
    Task<AttendanceResponse> CreateAsync(CreateAttendanceRequest request);
    Task<AttendanceResponse?> UpdateAsync(Guid id, UpdateAttendanceRequest request);
    Task<AttendanceSummaryResponse> GetSummaryAsync(AttendanceQueryRequest request);
    Task<List<AttendancePersonSummaryResponse>> GetStudentReportAsync(AttendanceQueryRequest request);
    Task<List<AttendancePersonSummaryResponse>> GetTeacherReportAsync(AttendanceQueryRequest request);
    Task<AttendanceResponse?> GetByEnrollmentDateAsync(
    Guid enrollmentId,
    DateTime date);
}
