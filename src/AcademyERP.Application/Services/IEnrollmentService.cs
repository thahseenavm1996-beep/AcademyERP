using AcademyERP.Application.DTOs.Enrollments;

namespace AcademyERP.Application.Services;

public interface IEnrollmentService
{
    Task<List<EnrollmentResponse>> GetAllAsync();

    Task<EnrollmentResponse?> GetByIdAsync(Guid id);

    Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request);

    Task<EnrollmentResponse?> UpdateAsync(
        Guid id,
        UpdateEnrollmentRequest request);

    Task<bool> DeleteAsync(Guid id);
}