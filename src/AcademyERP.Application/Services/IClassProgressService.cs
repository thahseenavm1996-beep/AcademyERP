using AcademyERP.Application.DTOs.ClassProgress;

namespace AcademyERP.Application.Services;

public interface IClassProgressService
{
    Task<ClassProgressResponse> CreateAsync(
        CreateClassProgressRequest request);

    Task<ClassProgressResponse?> GetByScheduledClassIdAsync(
        Guid scheduledClassId);
}