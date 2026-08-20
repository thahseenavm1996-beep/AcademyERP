using AcademyERP.Application.DTOs.ScheduledClasses;

namespace AcademyERP.Application.Services;

public interface IScheduledClassService
{
    Task<List<ScheduledClassResponse>> GetAllAsync();
    Task StartClassAsync(Guid id);
    Task<ScheduledClassResponse?> GetByIdAsync(Guid id);
    Task<ScheduledClassResponse?> UpdateAsync(
    Guid id,
    UpdateScheduledClassRequest request);
    Task CompleteClassAsync(
    Guid id,
    CompleteScheduledClassRequest request);
}