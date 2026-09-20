using AcademyERP.Application.DTOs.TeachingSchedules;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.TeachingSchedules;
using AcademyERP.Domain.Entities.ScheduledClasses;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.TeacherAvailabilities;

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

TeacherId = x.TeacherId,

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
var duration = await _context.ClassDurations
    .FirstOrDefaultAsync(d => d.Id == request.ClassDurationId);

if (duration == null)
    throw new Exception("Class duration not found.");

await ValidateTeacherAvailabilityAsync(
    request.TeacherId,
    request.DayOfWeek,
    request.StartTime,
    duration.Minutes);
    await ValidateTeacherScheduleConflictAsync(
    request.TeacherId,
    request.DayOfWeek,
    request.StartTime,
    duration.Minutes,
    request.EffectiveFrom,
    request.EffectiveTo);
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

        

        return new TeachingScheduleResponse
        {
            Id = schedule.Id,
TeacherId = schedule.TeacherId,
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

    private async Task ValidateTeacherScheduleConflictAsync(
    Guid teacherId,
    DayOfWeek dayOfWeek,
    TimeOnly startTime,
    int durationMinutes,
    DateTime effectiveFrom,
    DateTime? effectiveTo)
{
    var requestedStart = ToMinutes(startTime);
    var requestedEnd = requestedStart + durationMinutes;

    if (requestedEnd > 24 * 60)
    {
        throw new InvalidOperationException(
            $"The class starting at {startTime:hh\\:mm tt} " +
            $"cannot continue past midnight.");
    }

    var existingSchedules = await _context.TeachingSchedules
        .AsNoTracking()
        .Include(x => x.ClassDuration)
        .Where(x =>
            x.TeacherId == teacherId &&
            x.IsActive &&
            x.DayOfWeek == dayOfWeek)
        .ToListAsync();

    foreach (var existing in existingSchedules)
    {
        var existingStart = ToMinutes(existing.StartTime);
        var existingEnd =
            existingStart + existing.ClassDuration.Minutes;

        if (existingEnd > 24 * 60)
            existingEnd = 24 * 60;

        var datesOverlap =
            existing.EffectiveFrom.Date <=
                (effectiveTo ?? DateTime.MaxValue).Date
            &&
            (!existing.EffectiveTo.HasValue ||
             existing.EffectiveTo.Value.Date >= effectiveFrom.Date);

        if (!datesOverlap)
            continue;

        var timesOverlap =
            requestedStart < existingEnd &&
            requestedEnd > existingStart;

        if (!timesOverlap)
            continue;

        throw new InvalidOperationException(
            $"The selected teacher already has a class " +
            $"overlapping this time on {dayOfWeek}.");
    }
}

    private async Task ValidateTeacherAvailabilityAsync(
    Guid teacherId,
    DayOfWeek dayOfWeek,
    TimeOnly startTime,
    int durationMinutes)
{
    var allAvailability = await _context.TeacherAvailabilities
        .AsNoTracking()
        .Include(x => x.TimeSlot)
        .Where(x => x.TeacherId == teacherId && x.IsAvailable)
        .ToListAsync();

    // Backward compatibility:
    // Teachers with no availability configured can still be scheduled.
    if (allAvailability.Count == 0)
        return;

    var requestedStart = ToMinutes(startTime);
    var requestedEnd = requestedStart + durationMinutes;

    // Do not allow a scheduled class to run beyond midnight.
    if (requestedEnd > 24 * 60)
    {
        throw new InvalidOperationException(
            $"Teacher is not available on {dayOfWeek} from " +
            $"{startTime:hh\\:mm tt} for {durationMinutes} minutes.");
    }

    var daySlots = allAvailability
        .Where(x => x.DayOfWeek == dayOfWeek)
        .Select(x => new
        {
            Start = ToMinutes(x.TimeSlot.StartTime),
            End = ToMinutes(x.TimeSlot.EndTime)
        })
        .OrderBy(x => x.Start)
        .ToList();

    if (daySlots.Count == 0)
    {
        throw new InvalidOperationException(
            $"Teacher is not available on {dayOfWeek}.");
    }

    var coveredUntil = requestedStart;

    foreach (var slot in daySlots)
    {
        var slotStart = slot.Start;
        var slotEnd = slot.End;

        // Handle the 11 PM - 12 AM slot.
        if (slotEnd <= slotStart)
            slotEnd = 24 * 60;

        // This slot starts after the time we still need.
        // Therefore there is a gap.
        if (slotStart > coveredUntil)
            break;

        if (slotEnd > coveredUntil)
            coveredUntil = slotEnd;

        if (coveredUntil >= requestedEnd)
            return;
    }

    throw new InvalidOperationException(
        $"Teacher is not available on {dayOfWeek} from " +
        $"{startTime:hh\\:mm tt} to " +
        $"{TimeOnly.FromTimeSpan(
            TimeSpan.FromMinutes(requestedEnd)):hh\\:mm tt}.");
}

private static int ToMinutes(TimeOnly time)
{
    return (time.Hour * 60) + time.Minute;
}
}
