using AcademyERP.Application.DTOs.Reports;
using AcademyERP.Application.Services.Reports;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class BestPerformingFacultyService : IBestPerformingFacultyService
{
    private readonly ApplicationDbContext _context;

    public BestPerformingFacultyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BestPerformingFacultyReportResponse> GetReportAsync(
        string? courseName = null,
        Gender? gender = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        var schedulesQuery = _context.TeachingSchedules
            .AsNoTracking()
            .Include(x => x.Teacher)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .Where(x =>
                !x.IsDeleted &&
                x.Teacher != null);

        if (!string.IsNullOrWhiteSpace(courseName))
        {
            schedulesQuery = schedulesQuery
                .Where(x =>
                    x.Enrollment.Course.CourseName == courseName);
        }

        if (gender.HasValue)
        {
            schedulesQuery = schedulesQuery
                .Where(x =>
                    x.Teacher.Gender == gender.Value);
        }

        var schedules = await schedulesQuery.ToListAsync();

        var teacherIds = schedules
            .Select(x => x.TeacherId)
            .Distinct()
            .ToHashSet();

        if (teacherIds.Count == 0)
        {
            return new BestPerformingFacultyReportResponse();
        }

        var scheduledClassesQuery = _context.ScheduledClasses
            .AsNoTracking()
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Teacher)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Course)
            .Where(x =>
                !x.IsDeleted &&
                x.Status != ScheduledClassStatus.Cancelled &&
                teacherIds.Contains(
                    x.TeachingSchedule.TeacherId));

        if (fromDate.HasValue)
        {
            scheduledClassesQuery = scheduledClassesQuery
                .Where(x =>
                    x.ClassDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            scheduledClassesQuery = scheduledClassesQuery
                .Where(x =>
                    x.ClassDate <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(courseName))
        {
            scheduledClassesQuery = scheduledClassesQuery
                .Where(x =>
                    x.TeachingSchedule.Enrollment.Course.CourseName
                    == courseName);
        }

        var scheduledClasses =
            await scheduledClassesQuery.ToListAsync();

        var scheduledClassIds =
            scheduledClasses
                .Select(x => x.Id)
                .ToHashSet();

        var attendances = await _context.Attendances
            .AsNoTracking()
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
            .Where(x =>
                !x.IsDeleted &&
                scheduledClassIds.Contains(
                    x.ScheduledClassId))
            .ToListAsync();

        var classReports = await _context.ClassReports
            .AsNoTracking()
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Teacher)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Student)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Course)
            .Where(x =>
                !x.IsDeleted &&
                scheduledClassIds.Contains(
                    x.ScheduledClassId))
            .ToListAsync();

        var ranking = classReports
            .GroupBy(x => new
            {
                TeacherId =
                    x.ScheduledClass
                        .TeachingSchedule
                        .TeacherId,

                TeacherName =
                    x.ScheduledClass
                        .TeachingSchedule
                        .Teacher
                        .FullName,

                Gender =
                    x.ScheduledClass
                        .TeachingSchedule
                        .Teacher
                        .Gender
            })
            .Select(group =>
            {
                var teacherId = group.Key.TeacherId;

                var teacherAttendances =
                    attendances
                        .Where(x =>
                            x.ScheduledClass
                                .TeachingSchedule
                                .TeacherId == teacherId)
                        .ToList();

                var attendancePercentage =
                    teacherAttendances.Count == 0
                        ? 0
                        : teacherAttendances.Count(x =>
                              x.Status.ToString() == "Present" ||
                              x.Status.ToString() == "Late")
                          * 100.0 /
                          teacherAttendances.Count;

                var performance =
                    group.Count() == 0
                        ? 0
                        : group.Average(x =>
                            (double)x.PerformanceRating * 20);

                var courses =
                    group
                        .Select(x =>
                            x.ScheduledClass
                                .TeachingSchedule
                                .Enrollment
                                .Course
                                .CourseName)
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x))
                        .Distinct()
                        .OrderBy(x => x)
                        .ToList();

                var students =
                    group
                        .Select(x =>
                            x.ScheduledClass
                                .TeachingSchedule
                                .Enrollment
                                .StudentId)
                        .Distinct()
                        .Count();

                return new BestPerformingFacultyResponse
                {
                    TeacherId = teacherId,

                    TeacherName =
                        group.Key.TeacherName,

                    Gender =
                        group.Key.Gender.ToString(),

                    CourseName =
                        string.Join(", ", courses),

                    PerformanceScore =
                        performance,

                    AttendancePercentage =
                        attendancePercentage,

                    StudentsTaught =
                        students,

                    ClassesReported =
                        group
                            .Select(x =>
                                x.ScheduledClassId)
                            .Distinct()
                            .Count(),

                    AttendanceRecords =
                        teacherAttendances.Count
                };
            })
            .OrderByDescending(x =>
                x.PerformanceScore)
            .ThenByDescending(x =>
                x.AttendancePercentage)
            .ThenBy(x =>
                x.TeacherName)
            .Take(10)
            .ToList();

        for (var i = 0; i < ranking.Count; i++)
        {
            ranking[i].Rank = i + 1;
        }

        return new BestPerformingFacultyReportResponse
        {
            TotalFaculty =
                teacherIds.Count,

            FacultyWithReports =
                ranking.Count,

            AveragePerformance =
                ranking.Count == 0
                    ? 0
                    : ranking.Average(x =>
                        x.PerformanceScore),

            AverageAttendance =
                ranking.Count == 0
                    ? 0
                    : ranking.Average(x =>
                        x.AttendancePercentage),

            BestFaculty =
                ranking.FirstOrDefault(),

            Faculty =
                ranking
        };
    }
}
