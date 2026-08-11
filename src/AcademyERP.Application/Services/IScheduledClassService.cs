using AcademyERP.Application.DTOs.ScheduledClasses;

namespace AcademyERP.Application.Services;

public interface IScheduledClassService
{
    Task<List<ScheduledClassResponse>> GetAllAsync();
}