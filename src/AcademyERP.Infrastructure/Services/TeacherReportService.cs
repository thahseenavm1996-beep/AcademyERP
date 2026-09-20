using AcademyERP.Application.DTOs.TeacherReports;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class TeacherReportService
{
    private readonly ApplicationDbContext _context;

    public TeacherReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TeacherReportDashboardResponse> GetDashboardAsync()
    {
        var response = new TeacherReportDashboardResponse();

        var teachers = await _context.Teachers
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync();

        response.TotalTeachers = teachers.Count;

        response.ActiveTeachers = teachers.Count(
            x => x.Status == UserStatus.Active);

        var completedClasses = await _context.ScheduledClasses
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.Status == ScheduledClassStatus.Completed)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Teacher)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Course)
                        .ThenInclude(x => x.Program)
            .ToListAsync();

        response.CompletedClasses = completedClasses.Count;

        var classReports = await _context.ClassReports
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Teacher)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Student)
            .ToListAsync();

        if (classReports.Count > 0)
        {
            response.AveragePerformance =
                Math.Round(
                    classReports.Average(
                        x => (decimal)x.PerformanceRating * 20m),
                    2);

            response.PerformanceTrend = classReports
                .GroupBy(x => new
                {
                    x.ScheduledClass.ClassDate.Year,
                    x.ScheduledClass.ClassDate.Month
                })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new PerformanceTrendDto
                {
                    Month = new DateTime(
                        g.Key.Year,
                        g.Key.Month,
                        1)
                        .ToString("MMM yyyy"),

                    Score = Math.Round(
                        g.Average(
                            x => (decimal)x.PerformanceRating * 20m),
                        2)
                })
                .ToList();
        }

        var teacherReports = classReports
            .Where(x =>
                x.ScheduledClass.TeachingSchedule.Teacher != null)
            .GroupBy(x =>
                new
                {
                    TeacherId =
                        x.ScheduledClass.TeachingSchedule.Teacher.Id,

                    TeacherName =
                        x.ScheduledClass.TeachingSchedule.Teacher.FullName
                })
            .Select(g =>
            {
                var teacherId = g.Key.TeacherId;

                var teacherCompletedClasses =
                    completedClasses.Count(x =>
                        x.TeachingSchedule.TeacherId == teacherId);

                var studentCount = g
                    .Select(x =>
                        x.ScheduledClass
                            .TeachingSchedule
                            .Enrollment
                            .StudentId)
                    .Distinct()
                    .Count();

                var teacherAttendance =
                    _context.Attendances
                        .AsNoTracking()
                        .Where(a =>
                            !a.IsDeleted &&
                            a.ScheduledClass
                                .TeachingSchedule
                                .TeacherId == teacherId)
                        .ToList();

                var attendancePercentage =
                    teacherAttendance.Count == 0
                        ? 0
                        : Math.Round(
                            teacherAttendance.Count(a =>
                                a.Status == AttendanceStatus.Present ||
                                a.Status == AttendanceStatus.Late)
                            * 100m /
                            teacherAttendance.Count,
                            2);

                return new TeacherRankingDto
                {
                    Rank = 0,

                    TeacherName = g.Key.TeacherName,

                    PerformanceScore =
                        Math.Round(
                            g.Average(
                                x => (decimal)x.PerformanceRating * 20m),
                            2),

                    CompletedClasses =
                        teacherCompletedClasses,

                    AttendancePercentage =
                        attendancePercentage,

                    StudentsCount =
                        studentCount
                };
            })
            .OrderByDescending(x => x.PerformanceScore)
            .ThenByDescending(x => x.AttendancePercentage)
            .ThenByDescending(x => x.CompletedClasses)
            .ToList();

        for (var i = 0; i < teacherReports.Count; i++)
        {
            teacherReports[i].Rank = i + 1;
        }

        response.TeacherRanking = teacherReports;

        var bestTeacher = teacherReports.FirstOrDefault();

        if (bestTeacher != null)
        {
            var bestTeacherEntity = teachers.FirstOrDefault(
                x => x.FullName == bestTeacher.TeacherName);

            response.BestPerformingTeacher =
                new TeacherPerformanceDto
                {
                    TeacherId =
                        bestTeacherEntity?.Id ?? Guid.Empty,

                    TeacherName =
                        bestTeacher.TeacherName,

                    Score =
                        bestTeacher.PerformanceScore,

                    ClassesCompleted =
                        bestTeacher.CompletedClasses,

                    AttendancePercentage =
                        bestTeacher.AttendancePercentage
                };
        }

        response.JoiningTrend = teachers
            .GroupBy(x => new
            {
                x.JoiningDate.Year,
                x.JoiningDate.Month
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new JoiningTrendDto
            {
                Month = new DateTime(
                    g.Key.Year,
                    g.Key.Month,
                    1)
                    .ToString("MMM yyyy"),

                Count = g.Count()
            })
            .ToList();

       var teachingSchedules = await _context.TeachingSchedules
    .AsNoTracking()
    .Where(x =>
        !x.IsDeleted &&
        x.IsActive)
    .Include(x => x.Teacher)
    .Include(x => x.Enrollment)
        .ThenInclude(x => x.Student)
    .Include(x => x.ClassDuration)
    .Include(x => x.ScheduledClasses)
    .ToListAsync();

        response.TeacherWorkload = teachingSchedules
            .Where(x => x.Teacher != null)
            .GroupBy(x =>
                new
                {
                    TeacherId = x.TeacherId,
                    TeacherName = x.Teacher.FullName
                })
            .Select(g =>
            {
                var activeClasses = g.Count();

                var studentsCount = g
                    .Select(x => x.Enrollment.StudentId)
                    .Distinct()
                    .Count();

                var weeklyHours =
    g.Sum(x =>
        x.ClassDuration?.Minutes ?? 0)
    * 1m / 60m;

                return new TeacherWorkloadDto
                {
                    TeacherName =
                        g.Key.TeacherName,

                    StudentsCount =
                        studentsCount,

                    ActiveClasses =
                        activeClasses,

                    WeeklyHours =
                        Math.Round(
                            weeklyHours,
                            2)
                };
            })
            .OrderByDescending(x => x.ActiveClasses)
            .ToList();

        var programData = await _context.TeachingSchedules
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.IsActive)
            .Include(x => x.Teacher)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
                    .ThenInclude(x => x.Program)
            .ToListAsync();

        response.ProgramDistribution = programData
            .Where(x =>
                x.Teacher != null &&
                x.Enrollment?.Course?.Program != null)
            .GroupBy(x =>
                new
                {
                    ProgramId =
                        x.Enrollment.Course.ProgramId,

                    ProgramName =
                        x.Enrollment.Course.Program.ProgramName
                })
            .Select(g => new ProgramTeacherDistributionDto
            {
                ProgramName =
                    g.Key.ProgramName,

                TeachersCount =
                    g.Select(x => x.TeacherId)
                        .Distinct()
                        .Count(),

                StudentsCount =
                    g.Select(x =>
                        x.Enrollment.StudentId)
                        .Distinct()
                        .Count()
            })
            .OrderByDescending(x => x.TeachersCount)
            .ToList();

        return response;
    }
}