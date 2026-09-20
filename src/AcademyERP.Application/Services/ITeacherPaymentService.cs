using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.TeacherPayments;

namespace AcademyERP.Application.Services;

public interface ITeacherPaymentService
{
    Task<List<TeacherPaymentConfigurationResponse>> GetConfigurationsAsync(
        Guid? teacherId = null,
        Guid? enrollmentId = null);

    Task<TeacherPaymentConfigurationResponse?> GetConfigurationByIdAsync(
        Guid id);

    Task<TeacherPaymentConfigurationResponse> CreateConfigurationAsync(
        CreateTeacherPaymentConfigurationRequest request);

    Task<TeacherPaymentConfigurationResponse?> UpdateConfigurationAsync(
        Guid id,
        UpdateTeacherPaymentConfigurationRequest request);

    Task<bool> DeleteConfigurationAsync(Guid id);
    Task<TeacherPaymentConfigurationResponse?> StopConfigurationAsync(
    Guid id,
    DateTime stopDate);

    Task<TeacherPaymentConfigurationResponse?> PauseConfigurationAsync(
    Guid id,
    DateTime pauseStartDate,
    string? reason);

Task<TeacherPaymentConfigurationResponse?> ResumeConfigurationAsync(
    Guid id,
    DateTime resumeDate);

    Task<PagedResponse<TeacherPaymentResponse>> GetPaymentsAsync(
        TeacherPaymentQueryRequest request);

    Task<TeacherPaymentResponse?> GetPaymentByIdAsync(Guid id);

    Task<TeacherPaymentResponse?> MarkPaidAsync(
        Guid id,
        MarkTeacherPaymentPaidRequest request);

    Task<TeacherPaymentResponse?> CancelAsync(Guid id);

    Task<int> GeneratePendingCyclesAsync(
        DateTime asOfDate);
}
