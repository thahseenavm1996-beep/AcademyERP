using AcademyERP.Application.DTOs.TeacherPortal;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Enums;
using AcademyERP.Application.DTOs.ClassReports;

namespace AcademyERP.Infrastructure.Services;

public class TeacherPortalService : ITeacherPortalService
{
    private readonly ApplicationDbContext _context;


    public TeacherPortalService(
        ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<List<TeacherClassDto>> GetClassesAsync(
        Guid applicationUserId)
    {

        var teacherId =
            await _context.Teachers
            .Where(x =>
                x.ApplicationUserId == applicationUserId)

            .Select(x => x.Id)

            .FirstOrDefaultAsync();


        if (teacherId == Guid.Empty)
            return new List<TeacherClassDto>();



        return await _context.ScheduledClasses

            .Include(x =>
                x.TeachingSchedule)

                .ThenInclude(x =>
                    x.Enrollment)

                    .ThenInclude(x =>
                        x.Student)


            .Include(x =>
                x.TeachingSchedule)

                .ThenInclude(x =>
                    x.Enrollment)

                    .ThenInclude(x =>
                        x.Course)



            .Where(x =>
                x.TeachingSchedule.TeacherId == teacherId)



            .OrderBy(x => x.ClassDate)
.ThenBy(x => x.StartTime)



            .Select(x =>
                new TeacherClassDto
                {
                    Id = x.Id,

                    ClassDate =
                        x.ClassDate,


                    StudentName =
                        x.TeachingSchedule
                         .Enrollment
                         .Student
                         .FullName,


                    CourseName =
                        x.TeachingSchedule
                         .Enrollment
                         .Course
                         .CourseName,


                    StartTime =
                        x.StartTime,


                    EndTime =
                        x.EndTime,


                    Status =
    x.Status.ToString(),

AttendanceMarked =
    x.Attendances.Any()
                })


            .ToListAsync();
    }

    public async Task<List<TeacherStudentResponse>> GetStudentsAsync(
    Guid applicationUserId)
{
    var teacherId =
        await _context.Teachers
        .Where(x =>
            x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();


    if (teacherId == Guid.Empty)
        return new List<TeacherStudentResponse>();


    var students =
        await _context.TeachingSchedules

        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Student)
                .ThenInclude(x => x.StudentParents)
                    .ThenInclude(x => x.Parent)

        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Course)

        .Where(x =>
            x.TeacherId == teacherId)

        .Select(x => x.Enrollment)

        .Distinct()

        .ToListAsync();



    var result =
        new List<TeacherStudentResponse>();



    foreach(var enrollment in students)
    {
        var student =
            enrollment.Student;



        var parent =
            student.StudentParents
            .FirstOrDefault()
            ?.Parent;



        var attendanceRecords =
            await _context.Attendances

            .Where(a =>
                a.ScheduledClass
                .TeachingSchedule
                .EnrollmentId == enrollment.Id)

            .ToListAsync();



        var attendancePercentage = 0;


        if(attendanceRecords.Any())
        {
            var present =
                attendanceRecords.Count(x =>
                    x.Status == AttendanceStatus.Present
                    ||
                    x.Status == AttendanceStatus.Late);



            attendancePercentage =
                (int)Math.Round(
                    (double)present /
                    attendanceRecords.Count *
                    100);
        }



        result.Add(
            new TeacherStudentResponse
            {
                StudentId =
                    student.Id,


                StudentName =
                    student.FullName,


                CourseName =
                    enrollment.Course.CourseName,


                ParentName =
                    parent?.FullName
                    ?? "Not Available",


                Status =
                    student.Status.ToString(),


                AttendancePercentage =
                    attendancePercentage
            });
    }



    return result;
}
public async Task<List<TeacherClassDto>> GetScheduleAsync(
    Guid applicationUserId)
{
    var teacherId =
        await _context.Teachers
        .Where(x =>
            x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();


    if (teacherId == Guid.Empty)
        return new List<TeacherClassDto>();


    return await _context.ScheduledClasses

        .Include(x =>
            x.TeachingSchedule)
            .ThenInclude(x =>
                x.Enrollment)
                .ThenInclude(x =>
                    x.Student)

        .Include(x =>
            x.TeachingSchedule)
            .ThenInclude(x =>
                x.Enrollment)
                .ThenInclude(x =>
                    x.Course)


        .Where(x =>
            x.TeachingSchedule.TeacherId == teacherId)


        .OrderByDescending(x => x.ClassDate)
.ThenBy(x => x.StartTime)


        .Select(x => new TeacherClassDto
        {
            Id = x.Id,

            ClassDate =
                x.ClassDate,

            StudentName =
                x.TeachingSchedule
                .Enrollment
                .Student
                .FullName,

            CourseName =
                x.TeachingSchedule
                .Enrollment
                .Course
                .CourseName,

            StartTime =
                x.StartTime,

            EndTime =
                x.EndTime,

            Status =
                x.Status.ToString()
        })

        .ToListAsync();
}

public async Task<List<TeacherAttendanceDto>> GetAttendanceAsync(
    Guid applicationUserId)
{
    var teacherId =
        await _context.Teachers
        .Where(x => x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();


    if (teacherId == Guid.Empty)
        return new List<TeacherAttendanceDto>();


    return await _context.Attendances

        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)

        .Where(x =>
            x.ScheduledClass
            .TeachingSchedule
            .TeacherId == teacherId)

        .OrderByDescending(x =>
            x.ScheduledClass.ClassDate)

        .Select(x => new TeacherAttendanceDto
        {
            Date =
                x.ScheduledClass.ClassDate,

            StudentName =
                x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .Student
                .FullName,

            Status =
                x.Status.ToString()
        })

        .ToListAsync();
}

public async Task<TeacherAttendanceDto> MarkAttendanceAsync(
    Guid applicationUserId,
    Guid scheduledClassId,
    AttendanceStatus status,
    string? remarks)
{
    var teacherId = await _context.Teachers
        .Where(x => x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();

    if (teacherId == Guid.Empty)
        throw new UnauthorizedAccessException(
            "Teacher account not found.");

    var scheduledClass = await _context.ScheduledClasses
        .Include(x => x.TeachingSchedule)
            .ThenInclude(x => x.Enrollment)
                .ThenInclude(x => x.Student)
        .FirstOrDefaultAsync(x =>
            x.Id == scheduledClassId &&
            x.TeachingSchedule.TeacherId == teacherId);

    if (scheduledClass == null)
        throw new UnauthorizedAccessException(
            "This class is not assigned to you.");

    var exists = await _context.Attendances
        .AnyAsync(x =>
            x.ScheduledClassId == scheduledClassId);

    if (exists)
        throw new InvalidOperationException(
            "Attendance has already been marked for this class.");

    var attendance =
        new AcademyERP.Domain.Entities.Attendance.Attendance
        {
            ScheduledClassId = scheduledClassId,
            Status = status,
            Remarks = remarks
        };

    _context.Attendances.Add(attendance);

    await _context.SaveChangesAsync();

    return new TeacherAttendanceDto
    {
        Date = scheduledClass.ClassDate,
        StudentName =
            scheduledClass
                .TeachingSchedule
                .Enrollment
                .Student
                .FullName,
        Status = status.ToString()
    };
}

public async Task<List<TeacherReportDto>> GetReportsAsync(
    Guid applicationUserId)
{
    var teacherId =
        await _context.Teachers
            .Where(x =>
                x.ApplicationUserId == applicationUserId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

    if (teacherId == Guid.Empty)
        return new List<TeacherReportDto>();

    return await _context.ClassReports
        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)

        .Where(x =>
            x.ScheduledClass
                .TeachingSchedule
                .TeacherId == teacherId)

        .OrderByDescending(x =>
            x.ScheduledClass.ClassDate)

        .Select(x => new TeacherReportDto
        {
            ScheduledClassId = x.ScheduledClassId,
AttendanceStatus =
    x.ScheduledClass
        .Attendances
        .Select(a => a.Status)
        .FirstOrDefault(),
            StudentName =
                x.ScheduledClass
                    .TeachingSchedule
                    .Enrollment
                    .Student
                    .FullName,

            ClassDate =
                x.ScheduledClass.ClassDate,

            LessonTaken =
                x.LessonTaken,

            NextHomework =
                x.NextHomework,

            TeacherRemarks =
                x.TeacherRemarks ?? "",

            PerformanceRating =
                x.PerformanceRating,

            HomeworkStatus =
                x.HomeworkStatus,

            BehaviourRating =
                x.BehaviourRating,

            ClassOutcome =
                x.ClassOutcome,

            ActualDurationMinutes =
                x.ActualDurationMinutes
        })

        .ToListAsync();
}





    public async Task<CreateTeacherBulkReportsResponse> CreateBulkReportsAsync(
        Guid applicationUserId,
        CreateTeacherBulkReportsRequest request)
    {
        if (request.Reports == null || request.Reports.Count == 0)
            throw new InvalidOperationException(
                "There are no class reports to create.");

        var teacherId =
            await _context.Teachers
                .Where(x => x.ApplicationUserId == applicationUserId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

        if (teacherId == Guid.Empty)
            throw new UnauthorizedAccessException(
                "Teacher account not found.");

        var today = DateOnly.FromDateTime(DateTime.Now);

        var requestedIds = request.Reports
            .Select(x => x.ScheduledClassId)
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToList();

        if (requestedIds.Count == 0)
            throw new InvalidOperationException(
                "No valid scheduled classes were supplied.");

        var scheduledClasses =
            await _context.ScheduledClasses
                .Include(x => x.TeachingSchedule)
                .Where(x =>
                    requestedIds.Contains(x.Id) &&
                    x.TeachingSchedule.TeacherId == teacherId)
                .ToListAsync();

        if (scheduledClasses.Count != requestedIds.Count)
            throw new UnauthorizedAccessException(
                "One or more selected classes are not assigned to you.");

       var pastClassIds = scheduledClasses
    .Where(x => x.ClassDate < today)
    .Select(x => x.Id)
    .ToList();

var futureClassIds = scheduledClasses
    .Where(x => x.ClassDate > today)
    .Select(x => x.Id)
    .ToList();

if (futureClassIds.Count > 0)
{
    throw new InvalidOperationException(
        "Reports cannot be created for future classes.");
}

var approvedUnlockDates =
    await _context.ReportUnlockRequests
        .Where(x =>
            x.TeacherId == teacherId &&
            x.Status == ReportUnlockRequestStatus.Approved &&
            !x.IsDeleted)
        .Select(x => x.ReportDate)
        .ToHashSetAsync();

var lockedPastClasses = scheduledClasses
    .Where(x =>
        x.ClassDate < today &&
        !approvedUnlockDates.Contains(x.ClassDate))
    .Select(x => x.Id)
    .ToList();

if (lockedPastClasses.Count > 0)
{
    throw new InvalidOperationException(
        "One or more previous class reports are locked. " +
        "Please request an unlock from Admin before submitting them.");
}

        var existingIds =
            await _context.ClassReports
                .Where(x => requestedIds.Contains(x.ScheduledClassId))
                .Select(x => x.ScheduledClassId)
                .ToHashSetAsync();

       var uniqueRequests = request.Reports
    .GroupBy(x => x.ScheduledClassId)
    .Select(x => x.First())
    .ToList();

var skippedIds = uniqueRequests
    .Select(x => x.ScheduledClassId)
    .Where(x => existingIds.Contains(x))
    .Distinct()
    .ToList();

if (uniqueRequests.Count == 0)
{
    return new CreateTeacherBulkReportsResponse
    {
        CreatedCount = 0,
        SkippedCount = 0,
        SkippedScheduledClassIds = new List<Guid>()
    };
}

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var createdIds = new List<Guid>();

            foreach (var item in uniqueRequests)
{
    var scheduledClass =
        scheduledClasses.First(x =>
            x.Id == item.ScheduledClassId);

    if (!existingIds.Contains(item.ScheduledClassId))
    {
        var report =
            new AcademyERP.Domain.Entities.ClassReports.ClassReport
            {
                ScheduledClassId = item.ScheduledClassId,
                PerformanceRating = item.PerformanceRating,
                HomeworkStatus = item.HomeworkStatus,
                BehaviourRating = item.BehaviourRating,
                ClassOutcome = item.ClassOutcome,
                LessonTaken = item.LessonTaken,
                NextHomework = item.NextHomework,
                TeacherRemarks = item.TeacherRemarks,
                ActualDurationMinutes = item.ActualDurationMinutes,
                SubmittedAt = DateTime.UtcNow
            };

        _context.ClassReports.Add(report);

        createdIds.Add(report.Id);
    }

    var attendance =
        await _context.Attendances
            .FirstOrDefaultAsync(x =>
                x.ScheduledClassId == item.ScheduledClassId);

    if (attendance == null)
    {
        attendance =
            new AcademyERP.Domain.Entities.Attendance.Attendance
            {
                ScheduledClassId = item.ScheduledClassId,
                Status = item.AttendanceStatus
            };

        _context.Attendances.Add(attendance);
    }
    else
    {
        attendance.Status = item.AttendanceStatus;
    }

    scheduledClass.Status =
        ScheduledClassStatus.Completed;
}

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new CreateTeacherBulkReportsResponse
            {
                CreatedCount = createdIds.Count,
                SkippedCount = skippedIds.Count,
                CreatedReportIds = createdIds,
                SkippedScheduledClassIds = skippedIds
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


public async Task<TeacherProfileDto?> GetProfileAsync(


    Guid applicationUserId)
{
    return await _context.Teachers

        .Include(x => x.ApplicationUser)

        .Where(x =>
            x.ApplicationUserId == applicationUserId)

        .Select(x => new TeacherProfileDto
        {
            Name =
                x.FullName,

            Role =
                x.Department,

            Email =
                x.ApplicationUser.Email ?? "",

            Status =
                x.Status.ToString()
        })

        .FirstOrDefaultAsync();
}


public async Task<List<ClassReportResponse>> GetReportsForPdfAsync(
    Guid applicationUserId,
    DateTime? fromDate = null,
    DateTime? toDate = null,
    string? courseName = null,
    Gender? gender = null)
{
    var teacherId = await _context.Teachers
        .Where(x => x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();

    if (teacherId == Guid.Empty)
        return new List<ClassReportResponse>();

    var query = _context.ScheduledClasses
        .AsNoTracking()
        .Include(x => x.TeachingSchedule)
            .ThenInclude(x => x.Teacher)
        .Include(x => x.TeachingSchedule)
            .ThenInclude(x => x.Enrollment)
                .ThenInclude(x => x.Student)
        .Include(x => x.TeachingSchedule)
            .ThenInclude(x => x.Enrollment)
                .ThenInclude(x => x.Course)
        .Include(x => x.Attendances)
        .Where(x =>
            x.TeachingSchedule != null &&
            x.TeachingSchedule.TeacherId == teacherId);

    if (fromDate.HasValue)
    {
        query = query.Where(x =>
            x.ClassDate >= DateOnly.FromDateTime(fromDate.Value));
    }

    if (toDate.HasValue)
    {
        query = query.Where(x =>
            x.ClassDate <= DateOnly.FromDateTime(toDate.Value));
    }

    if (!string.IsNullOrWhiteSpace(courseName))
    {
        query = query.Where(x =>
            x.TeachingSchedule.Enrollment.Course.CourseName
                .Contains(courseName));
    }

    if (gender.HasValue)
    {
        query = query.Where(x =>
            x.TeachingSchedule.Enrollment.Student.Gender == gender.Value);
    }

    var scheduledClasses = await query
        .OrderByDescending(x => x.ClassDate)
        .ThenBy(x => x.StartTime)
        .ToListAsync();

    if (scheduledClasses.Count == 0)
        return new List<ClassReportResponse>();

    var scheduledClassIds = scheduledClasses
        .Select(x => x.Id)
        .ToList();

    // Get reports that actually exist for these scheduled classes.
    var reports = await _context.ClassReports
        .AsNoTracking()
        .Where(x => scheduledClassIds.Contains(x.ScheduledClassId))
        .ToDictionaryAsync(x => x.ScheduledClassId);

    var result = new List<ClassReportResponse>();

    foreach (var scheduledClass in scheduledClasses)
    {
        reports.TryGetValue(scheduledClass.Id, out var report);

       var attendance = scheduledClass.Attendances
    .OrderByDescending(a => a.ScheduledClass.ClassDate)
    .ThenByDescending(a => a.ScheduledClass.StartTime)
    .FirstOrDefault();

        var attendanceText = attendance?.Status.ToString() ?? "Not Marked";

        var response = new ClassReportResponse
        {
            Id = report?.Id ?? Guid.Empty,
            ScheduledClassId = scheduledClass.Id,
            TeacherId = teacherId,

            TeacherName =
                scheduledClass.TeachingSchedule.Teacher.FullName,

            StudentId =
                scheduledClass.TeachingSchedule.Enrollment.StudentId,

            StudentName =
                scheduledClass.TeachingSchedule.Enrollment.Student.FullName,

            CourseName =
                scheduledClass.TeachingSchedule.Enrollment.Course.CourseName,

            ClassDate = scheduledClass.ClassDate,

            // If no report exists, these values are only placeholders.
            // The PDF will display Nil/blank when the student is absent.
            PerformanceRating =
                report?.PerformanceRating ?? PerformanceRating.Poor,

            HomeworkStatus =
                report?.HomeworkStatus ?? HomeworkStatus.NotCompleted,

            BehaviourRating =
                report?.BehaviourRating ?? default,

            ClassOutcome =
                report?.ClassOutcome ?? default,

            LessonTaken =
                report?.LessonTaken ?? string.Empty,

            NextHomework =
                report?.NextHomework ?? string.Empty,

            TeacherRemarks =
                report?.TeacherRemarks,

            ActualDurationMinutes =
                report?.ActualDurationMinutes ?? 0,

            SubmittedAt =
                report?.SubmittedAt ?? default,

            IsLocked =
                report?.IsLocked ?? false,

            AttendanceStatusText = attendanceText
        };

        result.Add(response);
    }

    return result;
}
}