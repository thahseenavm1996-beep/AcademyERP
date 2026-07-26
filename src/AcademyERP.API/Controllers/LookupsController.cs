using AcademyERP.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LookupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LookupsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("timeslots")]
    public async Task<IActionResult> GetTimeSlots()
    {
        var result = await _context.TimeSlots
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.StartTime)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.StartTime,
                x.EndTime
            })
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("classdurations")]
    public async Task<IActionResult> GetClassDurations()
    {
        var result = await _context.ClassDurations
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Minutes)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Minutes
            })
            .ToListAsync();

        return Ok(result);
    }
}