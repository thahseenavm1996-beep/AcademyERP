using AcademyERP.Application.DTOs.TeacherRegistrations;

namespace AcademyERP.Application.Interfaces;

public interface ITeacherRegistrationService
{
    Task<TeacherRegistrationResponse> CreateAsync(
        CreateTeacherRegistrationRequest request);

    Task<List<TeacherRegistrationResponse>> GetAllAsync();

    Task<TeacherRegistrationResponse?> GetByIdAsync(Guid id);

    Task<List<TeacherRegistrationResponse>> GetPendingAsync();

    Task<bool> ApproveAsync(
        Guid id,
        ApproveTeacherRegistrationRequest request);

    Task<bool> RejectAsync(
        Guid id,
        string? reviewRemarks = null);
}
