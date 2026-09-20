using AcademyERP.Application.DTOs.ReportUnlockRequests;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Application.Interfaces;

public interface IReportUnlockRequestService
{
    Task<ReportUnlockRequestResponse> CreateAsync(
        Guid applicationUserId,
        CreateReportUnlockRequest request);

    Task<List<ReportUnlockRequestResponse>> GetForTeacherAsync(
        Guid applicationUserId);

    Task<List<ReportUnlockRequestResponse>> GetAllAsync(
        ReportUnlockRequestStatus? status = null);

    Task<bool> ReviewAsync(
        Guid requestId,
        Guid reviewedByUserId,
        ReportUnlockRequestStatus status,
        string? adminRemarks);
}
