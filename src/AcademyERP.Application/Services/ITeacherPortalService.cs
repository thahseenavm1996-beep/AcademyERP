using AcademyERP.Application.DTOs.TeacherPortal;
using AcademyERP.Domain.Enums;
using AcademyERP.Application.DTOs.ClassReports;


namespace AcademyERP.Application.Services;

public interface ITeacherPortalService
{

    Task<List<TeacherClassDto>> GetClassesAsync(
        Guid applicationUserId);


    Task<List<TeacherStudentResponse>> GetStudentsAsync(
        Guid applicationUserId);


    Task<List<TeacherClassDto>> GetScheduleAsync(
        Guid applicationUserId);


    Task<List<TeacherAttendanceDto>> GetAttendanceAsync(
        Guid applicationUserId);

Task<TeacherAttendanceDto> MarkAttendanceAsync(
    Guid applicationUserId,
    Guid scheduledClassId,
    AttendanceStatus status,
    string? remarks);

    Task<List<TeacherReportDto>> GetReportsAsync(
        Guid applicationUserId);

Task<List<ClassReportResponse>> GetReportsForPdfAsync(
    Guid applicationUserId,
    DateTime? fromDate = null,
    DateTime? toDate = null,
    string? courseName = null,
    Gender? gender = null);
    Task<CreateTeacherBulkReportsResponse> CreateBulkReportsAsync(
        Guid applicationUserId,
        CreateTeacherBulkReportsRequest request);


    Task<TeacherProfileDto?> GetProfileAsync(
        Guid applicationUserId);

}