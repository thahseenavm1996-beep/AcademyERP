using AcademyERP.Application.DTOs.Courses;

namespace AcademyERP.Application.Services;

public interface ICourseService
{
    Task<List<CourseResponse>> GetAllAsync();

    Task<CourseResponse?> GetByIdAsync(Guid id);

    Task<CourseResponse> CreateAsync(CreateCourseRequest request);

    Task<CourseResponse> UpdateAsync(Guid id, UpdateCourseRequest request);

    Task DeleteAsync(Guid id);
}