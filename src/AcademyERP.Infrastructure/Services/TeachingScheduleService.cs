using AcademyERP.Application.DTOs.TeachingSchedules;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.TeachingSchedules;
using AcademyERP.Domain.Entities.ScheduledClasses;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Infrastructure.Services;

public class TeachingScheduleService : ITeachingScheduleService
{
    private readonly ApplicationDbContext _context;

    public TeachingScheduleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeachingScheduleResponse>> GetAllAsync()
    {
        return await _context.TeachingSchedules
        .Include(x => x.Teacher)
            .Include(x => x.Enrollment)
                .ThenInclude(e => e.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(e => e.Teacher)
            .Include(x => x.Enrollment)
                .ThenInclude(e => e.Course)
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .Select(x => new TeachingScheduleResponse
            {
                Id = x.Id,

                StudentName = x.Enrollment.Student.FullName,

                TeacherName = x.Teacher.FullName,

                CourseName = x.Enrollment.Course.CourseName,

                DayOfWeek = x.DayOfWeek,

                StartTime = x.StartTime,

                EndTime = x.StartTime.Add(
        TimeSpan.FromMinutes(x.ClassDuration.Minutes))
            }).ToListAsync();
    }
    public async Task<TeachingScheduleResponse> CreateAsync(
    CreateTeachingScheduleRequest request)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == request.EnrollmentId);

        if (enrollment == null)
            throw new Exception("Enrollment not found.");

        var schedule = new TeachingSchedule
        {
            EnrollmentId = request.EnrollmentId,
            TeacherId = request.TeacherId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            ClassDurationId = request.ClassDurationId,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            IsActive = request.IsActive,
            MaximumStudents = request.MaximumStudents,
            Remarks = request.Remarks
        };

        _context.TeachingSchedules.Add(schedule);

        await _context.SaveChangesAsync();
        await GenerateScheduledClassesAsync(schedule);

        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.Id == request.TeacherId);

        var duration = await _context.ClassDurations
            .FirstOrDefaultAsync(d => d.Id == request.ClassDurationId);

        return new TeachingScheduleResponse
        {
            Id = schedule.Id,
            StudentName = enrollment.Student.FullName,
            TeacherName = teacher?.FullName ?? "",
            CourseName = enrollment.Course.CourseName,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            EndTime = schedule.StartTime.Add(
                TimeSpan.FromMinutes(duration?.Minutes ?? 0))
        };
    }
    private async Task GenerateScheduledClassesAsync(
    TeachingSchedule schedule)
    {
        var duration = await _context.ClassDurations
            .FirstAsync(x => x.Id == schedule.ClassDurationId);

        var firstDate = DateOnly.FromDateTime(schedule.EffectiveFrom.Date);

        while (firstDate.DayOfWeek != schedule.DayOfWeek)
        {
            firstDate = firstDate.AddDays(1);
        }

        DateOnly lastDate;

        if (schedule.EffectiveTo.HasValue)
        {
            lastDate = DateOnly.FromDateTime(schedule.EffectiveTo.Value.Date);
        }
        else
        {
            lastDate = DateOnly.FromDateTime(
    firstDate.ToDateTime(TimeOnly.MinValue).AddMonths(6));
        }

        var classes = new List<ScheduledClass>();

        for (var current = firstDate;
             current <= lastDate;
             current = current.AddDays(7))
        {
            classes.Add(new ScheduledClass
            {
                TeachingScheduleId = schedule.Id,

                ClassDate = current,

                StartTime = schedule.StartTime,

                EndTime = schedule.StartTime.Add(
                    TimeSpan.FromMinutes(duration.Minutes)),

                Status = ScheduledClassStatus.Scheduled
            });
        }

        _context.ScheduledClasses.AddRange(classes);

        await _context.SaveChangesAsync();
    }
}
