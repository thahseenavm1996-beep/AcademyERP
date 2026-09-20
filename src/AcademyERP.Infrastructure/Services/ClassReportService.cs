using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.ClassReports;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.ClassReports;
using AcademyERP.Persistence.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Enums;

namespace AcademyERP.Infrastructure.Services;

public class ClassReportService : IClassReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;


    public ClassReportService(
        ApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }



    public async Task<PagedResponse<ClassReportResponse>> GetAllAsync(
        ClassReportQueryRequest request)
    {

        var query = GetClassReportQuery();


        if (request.StudentId.HasValue)
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
        .TeachingSchedule
        .Enrollment
        .StudentId == request.StudentId.Value);
}


       if (request.TeacherId.HasValue)
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
        .TeachingSchedule
        .TeacherId == request.TeacherId.Value);
}

if (!string.IsNullOrWhiteSpace(request.StudentName))
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
            .TeachingSchedule
            .Enrollment
            .Student
            .FullName
            .Contains(request.StudentName));
}

if (!string.IsNullOrWhiteSpace(request.TeacherName))
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
            .TeachingSchedule
            .Teacher
            .FullName
            .Contains(request.TeacherName));
}
if (!string.IsNullOrWhiteSpace(request.CourseName))
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
            .TeachingSchedule
            .Enrollment
            .Course
            .CourseName == request.CourseName);
}

if (request.Gender.HasValue)
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
            .TeachingSchedule
            .Enrollment
            .Student
            .Gender == request.Gender.Value);
}

if (request.FromDate.HasValue)
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass.ClassDate >=
            DateOnly.FromDateTime(request.FromDate.Value));
}

if (request.ToDate.HasValue)
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass.ClassDate <=
            DateOnly.FromDateTime(request.ToDate.Value));
}

        var totalCount = await query.CountAsync();



        var reports = await query

    .OrderByDescending(x =>
        x.ScheduledClass != null
            ? x.ScheduledClass.ClassDate
            : DateOnly.MinValue)

    .Skip((request.Page - 1) * request.PageSize)

    .Take(request.PageSize)

    .ToListAsync();



        return new PagedResponse<ClassReportResponse>
        {
            Items = reports.Select(report =>
{
    var dto = _mapper.Map<ClassReportResponse>(report);

    dto.AttendanceStatusText =
        report.ScheduledClass.Attendances
            .FirstOrDefault()
            ?.Status
            .ToString()
            ?? "Not Marked";

    return dto;

}).ToList(),

            Page = request.Page,

            PageSize = request.PageSize,

            TotalCount = totalCount,

            TotalPages =
                (int)Math.Ceiling(
                    totalCount /
                    (double)request.PageSize)
        };
    }




    public async Task<ClassReportResponse?> GetByIdAsync(Guid id)
    {

        var report =
            await GetClassReportQuery()
            .FirstOrDefaultAsync(x => x.Id == id);



        if (report == null)
            return null;



        var dto = _mapper.Map<ClassReportResponse>(report);

dto.AttendanceStatusText =
    report.ScheduledClass.Attendances
        .FirstOrDefault()
        ?.Status
        .ToString()
        ?? "Not Marked";

return dto;
    }




    public async Task<ClassReportResponse> CreateAsync(
        CreateClassReportRequest request)
    {

var scheduledClass =
    await _context.ScheduledClasses
    .FirstOrDefaultAsync(x =>
        x.Id == request.ScheduledClassId);


if(scheduledClass == null)
{
    throw new Exception(
        "Scheduled class not found.");
}
       var exists =
    await _context.ClassReports.AnyAsync(x =>
        x.ScheduledClassId ==
        request.ScheduledClassId);



        if (exists)
        {
            throw new InvalidOperationException(
                "A class report already exists for this class.");
        }




        var report =
            new ClassReport
            {
                ScheduledClassId =
                    request.ScheduledClassId,


                PerformanceRating =
                    request.PerformanceRating,


                HomeworkStatus =
                    request.HomeworkStatus,


                BehaviourRating =
                    request.BehaviourRating,


                ClassOutcome =
                    request.ClassOutcome,


                LessonTaken =
                    request.LessonTaken,


                NextHomework =
                    request.NextHomework,


                TeacherRemarks =
                    request.TeacherRemarks,


                ActualDurationMinutes =
                    request.ActualDurationMinutes,


                SubmittedAt =
                    DateTime.UtcNow
            };



        _context.ClassReports.Add(report);


        await _context.SaveChangesAsync();



        var result =
            await GetByIdAsync(report.Id);



        return result ??
            throw new Exception(
                "Class report saved but retrieval failed.");
    }





    public async Task<ClassReportResponse?> UpdateAsync(
        Guid id,
        UpdateClassReportRequest request)
    {

        var report =
            await _context.ClassReports
            .FirstOrDefaultAsync(x =>
                x.Id == id);



        if (report == null)
            return null;



        report.PerformanceRating =
            request.PerformanceRating;


        report.HomeworkStatus =
            request.HomeworkStatus;


        report.BehaviourRating =
            request.BehaviourRating;


        report.ClassOutcome =
            request.ClassOutcome;


        report.LessonTaken =
            request.LessonTaken;


        report.NextHomework =
            request.NextHomework;


        report.TeacherRemarks =
            request.TeacherRemarks;


        report.ActualDurationMinutes =
            request.ActualDurationMinutes;



        await _context.SaveChangesAsync();



        return await GetByIdAsync(id);
    }




    public async Task<bool> DeleteAsync(Guid id)
    {

        var report =
            await _context.ClassReports
            .FirstOrDefaultAsync(x =>
                x.Id == id);



        if (report == null)
            return false;



        _context.ClassReports.Remove(report);


        await _context.SaveChangesAsync();



        return true;
    }





   private IQueryable<ClassReport> GetClassReportQuery()
{
    return _context.ClassReports

        .AsNoTracking()

        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Teacher)


        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)

            .Include(x => x.ScheduledClass)
    .ThenInclude(x => x.Attendances)

        .Include(x => x.ScheduledClass)
            .ThenInclude(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Course);
}

public async Task<ClassReportResponse?> GetByScheduledClassIdAsync(
    Guid scheduledClassId)
{
    var report =
        await GetClassReportQuery()
            .FirstOrDefaultAsync(x =>
                x.ScheduledClassId == scheduledClassId);

    if (report == null)
        return null;

    var dto = _mapper.Map<ClassReportResponse>(report);

    dto.AttendanceStatusText =
        report.ScheduledClass.Attendances
            .FirstOrDefault()
            ?.Status
            .ToString()
            ?? "Not Marked";

    return dto;
}

public async Task<List<ClassReportResponse>> GetForExportAsync(
    ClassReportQueryRequest request)
{
    var query = GetClassReportQuery();

    if (request.StudentId.HasValue)
    {
        query = query.Where(x =>
            x.ScheduledClass != null &&
            x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .StudentId == request.StudentId.Value);
    }

    if (request.TeacherId.HasValue)
    {
        query = query.Where(x =>
            x.ScheduledClass != null &&
            x.ScheduledClass
                .TeachingSchedule
                .TeacherId == request.TeacherId.Value);
    }

    if (!string.IsNullOrWhiteSpace(request.StudentName))
    {
        query = query.Where(x =>
            x.ScheduledClass != null &&
            x.ScheduledClass
                .TeachingSchedule
                .Enrollment
                .Student
                .FullName
                .Contains(request.StudentName));
    }

    if (!string.IsNullOrWhiteSpace(request.TeacherName))
    {
        query = query.Where(x =>
            x.ScheduledClass != null &&
            x.ScheduledClass
                .TeachingSchedule
                .Teacher
                .FullName
                .Contains(request.TeacherName));
    }
if (!string.IsNullOrWhiteSpace(request.CourseName))
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
            .TeachingSchedule
            .Enrollment
            .Course
            .CourseName
            .Contains(request.CourseName));
}

if (request.Gender.HasValue)
{
    query = query.Where(x =>
        x.ScheduledClass != null &&
        x.ScheduledClass
            .TeachingSchedule
            .Enrollment
            .Student
            .Gender == request.Gender.Value);
}
    if (request.FromDate.HasValue)
    {
        query = query.Where(x =>
            x.ScheduledClass != null &&
            x.ScheduledClass.ClassDate >=
                DateOnly.FromDateTime(request.FromDate.Value));
    }

    if (request.ToDate.HasValue)
    {
        query = query.Where(x =>
            x.ScheduledClass != null &&
            x.ScheduledClass.ClassDate <=
                DateOnly.FromDateTime(request.ToDate.Value));
    }

    if (request.AttendanceStatus.HasValue)
    {
        query = query.Where(x =>
            x.ScheduledClass != null &&
            x.ScheduledClass.Attendances.Any(a =>
                a.Status == request.AttendanceStatus.Value));
    }

    if (request.PerformanceRating.HasValue)
    {
        query = query.Where(x =>
            x.PerformanceRating ==
                request.PerformanceRating.Value);
    }

    var reports = await query
        .OrderByDescending(x =>
            x.ScheduledClass != null
                ? x.ScheduledClass.ClassDate
                : DateOnly.MinValue)
        .ToListAsync();

    return reports.Select(report =>
    {
        var dto = _mapper.Map<ClassReportResponse>(report);

        dto.AttendanceStatusText =
            report.ScheduledClass.Attendances
                .FirstOrDefault()
                ?.Status
                .ToString()
                ?? "Not Marked";

        return dto;
    }).ToList();
}

public async Task<List<TeacherReportMonitoringResponse>> GetTeacherReportMonitoringAsync(
    DateTime? fromDate,
    DateTime? toDate,
    Guid? teacherId = null,
    string? courseName = null,
    Gender? gender = null,
    bool? reportSubmitted = null)
{
    var query = _context.ScheduledClasses
    .AsNoTracking()
    .Include(x => x.TeachingSchedule)
        .ThenInclude(x => x.Enrollment)
            .ThenInclude(x => x.Student)
    .Include(x => x.TeachingSchedule)
        .ThenInclude(x => x.Enrollment)
            .ThenInclude(x => x.Course)
    .Where(x => true);

    if (fromDate.HasValue)
    {
        var from = DateOnly.FromDateTime(fromDate.Value);

        query = query.Where(x =>
            x.ClassDate >= from);
    }

    if (toDate.HasValue)
    {
        var to = DateOnly.FromDateTime(toDate.Value);

        query = query.Where(x =>
            x.ClassDate <= to);
    }

    if (teacherId.HasValue)
    {
        query = query.Where(x =>
            x.TeachingSchedule.TeacherId == teacherId.Value);
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

    var result = new List<TeacherReportMonitoringResponse>();

    foreach (var scheduledClass in scheduledClasses)
    {
        var report = await _context.ClassReports
            .AsNoTracking()
            .Where(x =>
                x.ScheduledClassId == scheduledClass.Id)
            .OrderByDescending(x => x.SubmittedAt)
            .FirstOrDefaultAsync();
var attendanceStatus =
    await _context.Attendances
        .AsNoTracking()
        .Where(x => x.ScheduledClassId == scheduledClass.Id)
        .Select(x => x.Status.ToString())
        .FirstOrDefaultAsync()
    ?? "Not Marked";

var reportRequired =
    !string.Equals(attendanceStatus, "Absent", StringComparison.OrdinalIgnoreCase) &&
    !string.Equals(attendanceStatus, "Leave", StringComparison.OrdinalIgnoreCase);

var isSubmitted = report is not null;
       

       if (reportSubmitted.HasValue)
{
    var effectiveReportSubmitted = reportRequired && isSubmitted;

    if (reportSubmitted.Value != effectiveReportSubmitted)
        continue;
}

        var teachingSchedule = scheduledClass.TeachingSchedule;
        var enrollment = teachingSchedule.Enrollment;
        var student = enrollment.Student;

        var teacher = await _context.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == teachingSchedule.TeacherId);

        result.Add(new TeacherReportMonitoringResponse
        {
            ScheduledClassId = scheduledClass.Id,

            ClassDate = scheduledClass.ClassDate,

            TeacherId = teachingSchedule.TeacherId,

            TeacherName = teacher?.FullName ?? string.Empty,
EmployeeCode = teacher?.EmployeeCode ?? string.Empty,

           StudentId = student?.Id ?? Guid.Empty,

StudentName = student?.FullName ?? string.Empty,

            CourseName =
    enrollment.Course?.CourseName ?? string.Empty,

            StartTime = scheduledClass.StartTime,
            EndTime = scheduledClass.EndTime,

           ClassStatus = scheduledClass.Status.ToString(),
AttendanceStatus = attendanceStatus,
ReportSubmitted = isSubmitted,

            ReportSubmittedAt =
                report?.SubmittedAt,

            ClassReportId =
                report?.Id
        });
    }

    return result;
}

}