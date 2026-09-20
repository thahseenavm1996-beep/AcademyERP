using AcademyERP.Application.DTOs.FeePaymentSubmissions;

namespace AcademyERP.Application.Interfaces;

public interface IFeePaymentSubmissionService
{
    Task<List<FeePaymentSubmissionResponse>> GetAllAsync();

    Task<List<FeePaymentSubmissionResponse>> GetByTeacherAsync(
        Guid applicationUserId);

    Task<FeePaymentSubmissionResponse?> GetByIdAsync(Guid id);

    Task<FeePaymentSubmissionResponse> SubmitAsync(
        Guid applicationUserId,
        CreateFeePaymentSubmissionRequest request);

    Task<FeePaymentSubmissionResponse?> ReviewAsync(
        Guid id,
        Guid reviewerId,
        ReviewFeePaymentSubmissionRequest request);

        Task<FeePaymentSubmissionResponse?> UploadProofAsync(
    Guid submissionId,
    Guid applicationUserId,
    Stream fileStream,
    string fileName,
    string contentType);
}