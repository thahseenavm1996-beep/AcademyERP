using AcademyERP.Application.DTOs.ParentPortal;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.Services;

public interface IParentPortalService
{
    Task<ParentDashboardResponse?> GetDashboardAsync(
        Guid applicationUserId,
        string parentName);

    Task<List<ParentChildSummaryDto>?> GetChildrenAsync(
        Guid applicationUserId);

    Task<List<ParentScheduleDto>?> GetScheduleAsync(
        Guid applicationUserId,
        Guid? studentId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null);

    Task<ParentAttendanceResponse?> GetAttendanceAsync(
        Guid applicationUserId,
        Guid? studentId = null,
        AttendanceStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null);

    Task<ParentFeesResponse?> GetFeesAsync(
        Guid applicationUserId,
        Guid? studentId = null);

    Task<List<ParentProgressDto>?> GetProgressAsync(
        Guid applicationUserId,
        Guid? studentId = null);

    Task<ParentProfileDto?> GetProfileAsync(
        Guid applicationUserId);

    Task<bool> UpdateProfileAsync(
        Guid applicationUserId,
        UpdateParentProfileRequest request);
}
