using AcademyERP.Application.DTOs.ScheduledClasses;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class ScheduledClassService : IScheduledClassService
{
    private readonly ApplicationDbContext _context;

    public ScheduledClassService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ScheduledClassResponse>> GetAllAsync()
    {
        return await _context.ScheduledClasses

            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)

            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Teacher)

            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Course)

            .OrderBy(x => x.ClassDate)

            .Select(x => new ScheduledClassResponse
            {
                Id = x.Id,

                ClassDate = x.ClassDate,

                StartTime = x.StartTime,

                EndTime = x.EndTime,

                StudentName =
                    x.TeachingSchedule.Enrollment.Student.FullName,

                TeacherName =
                    x.TeachingSchedule.Teacher.FullName,

                CourseName =
                    x.TeachingSchedule.Enrollment.Course.CourseName,

                Status = x.Status.ToString()
            })
            .ToListAsync();
    }
}