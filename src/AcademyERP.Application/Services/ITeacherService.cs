using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Teachers;

namespace AcademyERP.Application.Services;

public interface ITeacherService
{
    Task<TeacherResponse> CreateAsync(CreateTeacherRequest request);

    Task<PagedResponse<TeacherResponse>> GetAllAsync(TeacherQueryRequest request);

    Task<TeacherResponse?> GetByIdAsync(Guid id);

    Task<TeacherResponse?> UpdateAsync(Guid id, UpdateTeacherRequest request);

    Task<bool> DeleteAsync(Guid id);
}