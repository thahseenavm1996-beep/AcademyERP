using AcademyERP.Application.DTOs.Students;
using AcademyERP.Application.Common;
namespace AcademyERP.Application.Services;

public interface IStudentService
{
    Task<StudentResponse> CreateAsync(CreateStudentRequest request);

    Task<PagedResponse<StudentResponse>> GetAllAsync(StudentQueryRequest request);

    Task<StudentResponse?> GetByIdAsync(Guid id);

    Task<StudentResponse?> UpdateAsync(Guid id, UpdateStudentRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ResetPasswordAsync(Guid id, string newPassword);
}