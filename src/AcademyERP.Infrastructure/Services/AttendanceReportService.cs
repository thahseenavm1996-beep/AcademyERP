using AcademyERP.Application.DTOs.AttendanceReports;
using AcademyERP.Domain.Enums;
using AcademyERP.Application.Services;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class AttendanceReportService : IAttendanceReportService
{
    private readonly ApplicationDbContext _context;

    public AttendanceReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AttendanceReportDashboardResponse> GetDashboardAsync()
    {
        var response = new AttendanceReportDashboardResponse();

        // ---------------------------------------------------------
        // SCHEDULED CLASSES
        // ---------------------------------------------------------

        var scheduledClasses = await _context.ScheduledClasses
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Course)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Teacher)
            .ToListAsync();

        response.TotalClasses = scheduledClasses.Count;

        response.CompletedClasses = scheduledClasses.Count(
            x => x.Status == ScheduledClassStatus.Completed);


        // ---------------------------------------------------------
        // ATTENDANCE RECORDS
        // ---------------------------------------------------------

        var attendance = await _context.Attendances
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
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
            .ToListAsync();

        response.TotalAttendanceRecords = attendance.Count;


        // ---------------------------------------------------------
        // OVERALL ATTENDANCE
        // ---------------------------------------------------------

        if (attendance.Count > 0)
        {
            response.AverageAttendance =
                Math.Round(
                    attendance.Count(x =>
                        x.Status == AttendanceStatus.Present ||
                        x.Status == AttendanceStatus.Late)
                    * 100m / attendance.Count,
                    2);
        }


        // ---------------------------------------------------------
        // ATTENDANCE TREND
        // ---------------------------------------------------------

        response.AttendanceTrend = attendance
            .GroupBy(x => new
            {
                x.ScheduledClass.ClassDate.Year,
                x.ScheduledClass.ClassDate.Month
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g =>
            {
                var total = g.Count();

                var attended = g.Count(x =>
                    x.Status == AttendanceStatus.Present ||
                    x.Status == AttendanceStatus.Late);

                return new AttendanceTrendDto
                {
                    Month = new DateTime(
                        g.Key.Year,
                        g.Key.Month,
                        1).ToString("MMM yyyy"),

                    Percentage = total == 0
                        ? 0
                        : Math.Round(
                            attended * 100m / total,
                            2)
                };
            })
            .ToList();


        // ---------------------------------------------------------
        // STATUS DISTRIBUTION
        // ---------------------------------------------------------

        response.StatusDistribution = attendance
            .GroupBy(x => x.Status)
            .Select(g => new AttendanceStatusDistributionDto
            {
                Status = g.Key.ToString(),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToList();


        // ---------------------------------------------------------
        // COURSE ATTENDANCE
        // ---------------------------------------------------------

        response.CourseAttendance = attendance
            .Where(x =>
                x.ScheduledClass.TeachingSchedule.Enrollment.Course != null)
            .GroupBy(x =>
                new
                {
                    CourseId =
                        x.ScheduledClass
                            .TeachingSchedule
                            .Enrollment
                            .CourseId,

                    CourseName =
                        x.ScheduledClass
                            .TeachingSchedule
                            .Enrollment
                            .Course
                            .CourseName
                })
            .Select(g =>
            {
                var total = g.Count();

                var attended = g.Count(x =>
                    x.Status == AttendanceStatus.Present ||
                    x.Status == AttendanceStatus.Late);

                return new CourseAttendanceDto
                {
                    CourseName = g.Key.CourseName,

                    AttendancePercentage = total == 0
                        ? 0
                        : Math.Round(
                            attended * 100m / total,
                            2)
                };
            })
            .OrderByDescending(x => x.AttendancePercentage)
            .ToList();


        // ---------------------------------------------------------
        // STUDENTS NEEDING ATTENTION
        // ---------------------------------------------------------

        response.StudentsNeedingAttention = attendance
            .Where(x =>
                x.ScheduledClass.TeachingSchedule.Enrollment.Student != null &&
                x.ScheduledClass.TeachingSchedule.Enrollment.Course != null)
            .GroupBy(x =>
                new
                {
                    StudentId =
                        x.ScheduledClass
                            .TeachingSchedule
                            .Enrollment
                            .StudentId,

                    StudentName =
                        x.ScheduledClass
                            .TeachingSchedule
                            .Enrollment
                            .Student
                            .FullName,

                    CourseId =
                        x.ScheduledClass
                            .TeachingSchedule
                            .Enrollment
                            .CourseId,

                    CourseName =
                        x.ScheduledClass
                            .TeachingSchedule
                            .Enrollment
                            .Course
                            .CourseName
                })
            .Select(g =>
            {
                var total = g.Count();

                var attended = g.Count(x =>
                    x.Status == AttendanceStatus.Present ||
                    x.Status == AttendanceStatus.Late);

                var percentage = total == 0
                    ? 0
                    : Math.Round(
                        attended * 100m / total,
                        2);

                return new StudentAttendanceAlertDto
                {
                    StudentName = g.Key.StudentName,
                    CourseName = g.Key.CourseName,
                    AttendancePercentage = percentage
                };
            })
            .Where(x => x.AttendancePercentage < 75m)
            .OrderBy(x => x.AttendancePercentage)
            .ThenBy(x => x.StudentName)
            .Take(10)
            .ToList();


        // ---------------------------------------------------------
        // TEACHER ATTENDANCE SUMMARY
        // ---------------------------------------------------------

        var teacherAttendanceLookup = attendance
            .Where(x =>
                x.ScheduledClass.TeachingSchedule.Teacher != null)
            .GroupBy(x =>
                x.ScheduledClass
                    .TeachingSchedule
                    .TeacherId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    TotalAttendanceRecords = g.Count(),

                    Attended = g.Count(x =>
                        x.Status == AttendanceStatus.Present ||
                        x.Status == AttendanceStatus.Late)
                });


        response.TeacherSummary = scheduledClasses
            .Where(x =>
                x.TeachingSchedule.Teacher != null)
            .GroupBy(x =>
                new
                {
                    TeacherId =
                        x.TeachingSchedule.TeacherId,

                    TeacherName =
                        x.TeachingSchedule.Teacher.FullName
                })
            .Select(g =>
            {
                teacherAttendanceLookup.TryGetValue(
                    g.Key.TeacherId,
                    out var teacherAttendance);

                var totalAttendanceRecords =
                    teacherAttendance?.TotalAttendanceRecords ?? 0;

                var attended =
                    teacherAttendance?.Attended ?? 0;

                var attendancePercentage =
                    totalAttendanceRecords == 0
                        ? 0
                        : Math.Round(
                            attended * 100m /
                            totalAttendanceRecords,
                            2);

                return new TeacherAttendanceSummaryDto
                {
                    TeacherName = g.Key.TeacherName,

                    TotalClasses = g.Count(),

                    AttendancePercentage =
                        attendancePercentage
                };
            })
            .OrderByDescending(x => x.AttendancePercentage)
            .ThenBy(x => x.TeacherName)
            .ToList();


        return response;
    }
}