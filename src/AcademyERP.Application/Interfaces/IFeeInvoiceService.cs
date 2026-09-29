using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Fees;
using AcademyERP.Application.DTOs.Enrollments;

namespace AcademyERP.Application.Interfaces;

public interface IFeeInvoiceService
{
    Task<PagedResponse<FeeInvoiceResponse>> GetAllAsync(
        FeeInvoiceQueryRequest request);

    Task<List<FeeInvoiceResponse>> GetAllForExportAsync(
        FeeInvoiceQueryRequest request);

    Task<FeeInvoiceResponse?> GetByIdAsync(Guid id);

    Task<FeeInvoiceResponse> CreateAsync(
        CreateFeeInvoiceRequest request);

    Task<FeeInvoiceResponse?> UpdateAsync(
        Guid id,
        UpdateFeeInvoiceRequest request);

    Task<bool> DeleteAsync(Guid id);

    Task<int> GeneratePendingCyclesAsync(DateTime asOfDate);

    Task<EnrollmentResponse?> PauseEnrollmentAsync(
        Guid enrollmentId,
        DateTime pauseStartDate,
        string? reason);

    Task<EnrollmentResponse?> ResumeEnrollmentAsync(
        Guid enrollmentId,
        DateTime resumeDate);

    Task<List<FeeInvoiceResponse>> GetTeacherOutstandingInvoicesAsync(
        Guid applicationUserId);
}
