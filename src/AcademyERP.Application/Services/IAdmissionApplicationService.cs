using AcademyERP.Application.DTOs.Admissions;
using AcademyERP.Domain.Enums;
namespace AcademyERP.Application.Services;

public interface IAdmissionApplicationService
{
    Task<AdmissionApplicationResponse> CreateAsync(
        CreateAdmissionApplicationRequest request);

    Task<List<AdmissionApplicationResponse>> GetAllAsync();

    Task<AdmissionApplicationResponse?> GetByIdAsync(Guid id);
    Task<AdmissionApplicationResponse?> UpdateStatusAsync(
    Guid id,
    AdmissionStatus status);
}