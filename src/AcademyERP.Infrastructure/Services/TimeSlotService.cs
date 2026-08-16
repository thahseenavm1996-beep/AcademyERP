using AcademyERP.Application.DTOs.Lookups;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class TimeSlotService : ITimeSlotService
{
    private readonly ApplicationDbContext _context;

    public TimeSlotService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TimeSlotResponse>>
        GetAllAsync()
    {
        return await _context.TimeSlots
            .Where(x => x.IsActive)
            .Select(x => new TimeSlotResponse
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }
}