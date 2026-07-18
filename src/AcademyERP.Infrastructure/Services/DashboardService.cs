using AcademyERP.Application.DTOs.Dashboard;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync()
    {
        return new DashboardSummaryResponse
        {
            TotalStudents = await _context.Students.CountAsync(),
            TotalTeachers = await _context.Teachers.CountAsync(),
            TotalParents = await _context.Parents.CountAsync(),
            TotalPrograms = await _context.Programs.CountAsync(),

            ActiveStudents = await _context.Students.CountAsync(x => x.Status == UserStatus.Active),
            ActiveTeachers = await _context.Teachers.CountAsync(x => x.Status == UserStatus.Active)
        };
    }
}