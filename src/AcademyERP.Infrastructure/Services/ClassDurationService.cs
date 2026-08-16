using AcademyERP.Application.DTOs.Lookups;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class ClassDurationService : IClassDurationService
{
    private readonly ApplicationDbContext _context;

    public ClassDurationService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassDurationResponse>>
        GetAllAsync()
    {
        return await _context.ClassDurations
            .Where(x => x.IsActive)
            .Select(x => new ClassDurationResponse
            {
                Id = x.Id,
                Name = x.Name,
                Minutes = x.Minutes
            })
            .ToListAsync();
    }
}