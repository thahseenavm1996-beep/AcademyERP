using AcademyERP.Application.DTOs.Reports;
using AcademyERP.Application.Services.Reports;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class BestPerformingStudentService : IBestPerformingStudentService
{
    private readonly ApplicationDbContext _context;

    public BestPerformingStudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BestPerformingStudentsReportResponse> GetReportAsync(
        string? courseName = null,
        Gender? gender = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        var enrollmentsQuery = _context.Enrollments
            .AsNoTracking()
            .Include(x => x.Student)
            .Include(x => x.Course)
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(courseName))
        {
            enrollmentsQuery = enrollmentsQuery
                .Where(x => x.Course.CourseName == courseName);
        }

        if (gender.HasValue)
        {
            enrollmentsQuery = enrollmentsQuery
                .Where(x => x.Student.Gender == gender.Value);
        }

        var enrollments = await enrollmentsQuery.ToListAsync();

        var studentIds = enrollments
            .Select(x => x.StudentId)
            .Distinct()
            .ToHashSet();

        if (studentIds.Count == 0)
        {
            return new BestPerformingStudentsReportResponse();
        }

    var scheduledClassesQuery = _context.ScheduledClasses
        .AsNoTracking()
        .Include(x => x.TeachingSchedule)
            .ThenInclude(x => x.Enrollment)
                .ThenInclude(x => x.Student)
        .Include(x => x.TeachingSchedule)
            .ThenInclude(x => x.Enrollment)
                .ThenInclude(x => x.Course)
        .Include(x => x.TeachingSchedule)
            .ThenInclude(x => x.Teacher)
        .Where(x =>
            !x.IsDeleted &&
            x.Status != ScheduledClassStatus.Cancelled &&
            studentIds.Contains(x.TeachingSchedule.Enrollment.StudentId));

        if (fromDate.HasValue)
        {
            scheduledClassesQuery = scheduledClassesQuery
                .Where(x => x.ClassDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            scheduledClassesQuery = scheduledClassesQuery
                .Where(x => x.ClassDate <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(courseName))
        {
            scheduledClassesQuery = scheduledClassesQuery
                .Where(x => x.TeachingSchedule.Enrollment.Course.CourseName == courseName);
        }

        var scheduledClasses = await scheduledClassesQuery.ToListAsync();

        var classIds = scheduledClasses
            .Select(x => x.Id)
            .ToHashSet();

        var attendanceQuery = _context.Attendances
            .AsNoTracking()
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
            .Where(x =>
                !x.IsDeleted &&
                classIds.Contains(x.ScheduledClassId));

        var attendances = await attendanceQuery.ToListAsync();

        var classReportsQuery = _context.ClassReports
            .AsNoTracking()
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Student)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Course)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Teacher)
            .Where(x =>
                !x.IsDeleted &&
                classIds.Contains(x.ScheduledClassId));

        var classReports = await classReportsQuery.ToListAsync();

        var ranking = classReports
            .GroupBy(x => new
            {
                x.ScheduledClass.TeachingSchedule.Enrollment.StudentId,
                StudentName = x.ScheduledClass.TeachingSchedule.Enrollment.Student.FullName,
                Gender = x.ScheduledClass.TeachingSchedule.Enrollment.Student.Gender
            })
            .Select(group =>
            {
                var studentId = group.Key.StudentId;

                var studentAttendance = attendances
                    .Where(x =>
                        x.ScheduledClass.TeachingSchedule.Enrollment.StudentId == studentId)
                    .ToList();

                var attendancePercentage =
                    studentAttendance.Count == 0
                        ? 0
                        : studentAttendance.Count(x =>
                              x.Status.ToString() == "Present" ||
                              x.Status.ToString() == "Late")
                          * 100.0
                          / studentAttendance.Count;

                var performance =
                    group.Count() == 0
                        ? 0
                        : group.Average(x => (double)x.PerformanceRating * 20);

                var courses = group
                    .Select(x => x.ScheduledClass.TeachingSchedule.Enrollment.Course.CourseName)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                return new BestPerformingStudentResponse
                {
                    StudentId = studentId,
                    StudentName = group.Key.StudentName,
                    Gender = group.Key.Gender.ToString(),
                    TeacherName = string.Join(", ", group
                        .Select(x => x.ScheduledClass.TeachingSchedule.Teacher.FullName)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x)),
                    CourseName = string.Join(", ", courses),
                    PerformanceScore = performance,
                    AttendancePercentage = attendancePercentage,
                    ClassesReported = group.Count(),
                    AttendanceRecords = studentAttendance.Count
                };
            })
            .OrderByDescending(x => x.PerformanceScore)
            .ThenByDescending(x => x.AttendancePercentage)
            .ThenBy(x => x.StudentName)
            .Take(10)
            .ToList();

        for (var i = 0; i < ranking.Count; i++)
        {
            ranking[i].Rank = i + 1;
        }

        return new BestPerformingStudentsReportResponse
        {
            TotalStudents = studentIds.Count,
            StudentsWithReports = ranking.Count,
            AveragePerformance = ranking.Count == 0
                ? 0
                : ranking.Average(x => x.PerformanceScore),
            AverageAttendance = ranking.Count == 0
                ? 0
                : ranking.Average(x => x.AttendancePercentage),
            BestStudent = ranking.FirstOrDefault(),
            Students = ranking
        };
    }
}
