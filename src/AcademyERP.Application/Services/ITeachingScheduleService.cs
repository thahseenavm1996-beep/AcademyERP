using AcademyERP.Application.DTOs.TeachingSchedules;

namespace AcademyERP.Application.Services;

public interface ITeachingScheduleService
{
    Task<List<TeachingScheduleResponse>> GetAllAsync();

    Task<TeachingScheduleResponse> CreateAsync(
        CreateTeachingScheduleRequest request);
}