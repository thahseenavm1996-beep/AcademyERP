using AcademyERP.Application.DTOs.Lookups;

namespace AcademyERP.Application.Services;

public interface ITimeSlotService
{
    Task<List<TimeSlotResponse>> GetAllAsync();
}