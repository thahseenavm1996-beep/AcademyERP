using AcademyERP.Application.DTOs.ScheduledClasses;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Enums;
using AcademyERP.Domain.Entities.Attendance;

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

    TeachingScheduleId = x.TeachingScheduleId,
    EnrollmentId =
    x.TeachingSchedule.EnrollmentId,

StudentId =
    x.TeachingSchedule.Enrollment.StudentId,

TeacherId =
    x.TeachingSchedule.TeacherId,
    ClassDate = x.ClassDate,

    StartTime = x.StartTime,

    EndTime = x.EndTime,


    StudentName =
        x.TeachingSchedule.Enrollment.Student.FullName,

    TeacherName =
        x.TeachingSchedule.Teacher.FullName,

    CourseName =
        x.TeachingSchedule.Enrollment.Course.CourseName,


    Status = x.Status.ToString(),

    ActualStartTime = x.ActualStartTime,

    ActualEndTime = x.ActualEndTime,

    CancellationReason = x.CancellationReason,

   Remarks = x.Remarks,

AttendanceStatus =
    _context.Attendances
        .Where(a => a.ScheduledClassId == x.Id)
        .Select(a => a.Status.ToString())
        .FirstOrDefault()
        ?? "Not Marked",

ClassReportStatus =
    _context.ClassReports
        .Any(r => r.ScheduledClassId == x.Id)
        ? "Sent"
        : "Not Sent"
})
            .ToListAsync();
    }
    public async Task StartClassAsync(Guid id)
{
    var scheduledClass =
        await _context.ScheduledClasses
        .FirstOrDefaultAsync(x => x.Id == id);


    if (scheduledClass == null)
    {
        throw new Exception(
            "Scheduled class not found.");
    }


    scheduledClass.Status =
        ScheduledClassStatus.Started;


    scheduledClass.ActualStartTime =
        DateTime.UtcNow;


    await _context.SaveChangesAsync();
}
public async Task CompleteClassAsync(
    Guid id,
    CompleteScheduledClassRequest request)
{
    var scheduledClass =
        await _context.ScheduledClasses
        .FirstOrDefaultAsync(x => x.Id == id);


    if (scheduledClass == null)
        throw new Exception(
            "Scheduled class not found.");


    scheduledClass.Status =
        ScheduledClassStatus.Completed;


    scheduledClass.ActualEndTime =
        DateTime.UtcNow;


    await _context.SaveChangesAsync();
}
public async Task<ScheduledClassResponse?> GetByIdAsync(Guid id)
{
    return await _context.ScheduledClasses
        .Where(x => x.Id == id)
        .Select(x => new ScheduledClassResponse
        {
            Id = x.Id,

            TeachingScheduleId = x.TeachingScheduleId,
            EnrollmentId =
    x.TeachingSchedule.EnrollmentId,

StudentId =
    x.TeachingSchedule.Enrollment.StudentId,

TeacherId =
    x.TeachingSchedule.TeacherId,

            ClassDate = x.ClassDate,

            StartTime = x.StartTime,

            EndTime = x.EndTime,

            StudentName =
                x.TeachingSchedule.Enrollment.Student.FullName,

            TeacherName =
                x.TeachingSchedule.Teacher.FullName,

            CourseName =
                x.TeachingSchedule.Enrollment.Course.CourseName,

            Status = x.Status.ToString(),

            ActualStartTime = x.ActualStartTime,

            ActualEndTime = x.ActualEndTime,

            CancellationReason = x.CancellationReason,

            Remarks = x.Remarks,

AttendanceStatus =
    _context.Attendances
        .Where(a => a.ScheduledClassId == x.Id)
        .Select(a => a.Status.ToString())
        .FirstOrDefault()
        ?? "Not Marked",

ClassReportStatus =
    _context.ClassReports
        .Any(r => r.ScheduledClassId == x.Id)
        ? "Sent"
        : "Not Sent"

        })
        .FirstOrDefaultAsync();
}
public async Task<ScheduledClassResponse?> UpdateAsync(
    Guid id,
    UpdateScheduledClassRequest request)
{
    var scheduledClass =
        await _context.ScheduledClasses
        .FirstOrDefaultAsync(x => x.Id == id);


    if (scheduledClass == null)
        return null;


    scheduledClass.Status = request.Status;

    scheduledClass.CancellationReason =
        request.CancellationReason;

    scheduledClass.Remarks =
        request.Remarks;


    await _context.SaveChangesAsync();


    return await GetByIdAsync(id);
}

public async Task MarkStudentAbsentAsync(Guid id)
{
    var scheduledClass =
        await _context.ScheduledClasses
            .FirstOrDefaultAsync(x => x.Id == id);

    if (scheduledClass == null)
    {
        throw new Exception(
            "Scheduled class not found.");
    }

    if (scheduledClass.Status !=
        ScheduledClassStatus.Scheduled)
    {
        throw new InvalidOperationException(
            "Only scheduled classes can be marked as absent.");
    }

    var attendance =
        await _context.Attendances
            .FirstOrDefaultAsync(x =>
                x.ScheduledClassId == id);

    if (attendance == null)
    {
        attendance = new Attendance
        {
            ScheduledClassId = id,
            Status = AttendanceStatus.Absent,
            Remarks = "Student was absent."
        };

        _context.Attendances.Add(attendance);
    }
    else
    {
        attendance.Status =
            AttendanceStatus.Absent;

        attendance.Remarks =
            "Student was absent.";
    }

    scheduledClass.Status =
        ScheduledClassStatus.Missed;

    scheduledClass.CancellationReason =
        "Student absent";

    await _context.SaveChangesAsync();
}
}