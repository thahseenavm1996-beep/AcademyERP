using AcademyERP.Application.DTOs.StudentReports;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class StudentReportDashboardService
{
    private readonly ApplicationDbContext _context;

    public StudentReportDashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

   public async Task<StudentReportDashboardResponse> GetDashboardAsync(
    string? courseName = null,
    Gender? gender = null)
{
    var response = new StudentReportDashboardResponse();

    // ---------------------------------------------------------
    // Enrollments / Courses / Student filtering
    // ---------------------------------------------------------

    var enrollments = await _context.Enrollments
        .AsNoTracking()
        .Where(x => !x.IsDeleted)
        .Include(x => x.Student)
        .Include(x => x.Course)
        .ToListAsync();

    if (!string.IsNullOrWhiteSpace(courseName))
    {
        enrollments = enrollments
            .Where(x =>
                x.Course != null &&
                x.Course.CourseName == courseName)
            .ToList();
    }

    if (gender.HasValue)
    {
        enrollments = enrollments
            .Where(x =>
                x.Student != null &&
                x.Student.Gender == gender.Value)
            .ToList();
    }

    var filteredStudentIds = enrollments
        .Where(x => x.Student != null)
        .Select(x => x.StudentId)
        .Distinct()
        .ToHashSet();

    // ---------------------------------------------------------
    // Students
    // ---------------------------------------------------------

    var students = await _context.Students
        .AsNoTracking()
        .Where(x =>
            !x.IsDeleted &&
            filteredStudentIds.Contains(x.Id))
        .ToListAsync();

    response.TotalStudents = students.Count;

    response.ActiveStudents = students.Count(
        x => x.Status == UserStatus.Active);

    response.StudentStatus = students
        .GroupBy(x => x.Status)
        .Select(g => new StudentStatusDto
        {
            Status = g.Key.ToString(),
            Count = g.Count()
        })
        .OrderByDescending(x => x.Count)
        .ToList();

    // ---------------------------------------------------------
    // Course Distribution
    // ---------------------------------------------------------

    response.CourseDistribution = enrollments
        .Where(x => x.Student != null && x.Course != null)
        .GroupBy(x => x.Course.CourseName)
        .Select(g => new CourseStudentDistributionDto
        {
            CourseName = g.Key,
            Students = g.Select(x => x.StudentId)
                .Distinct()
                .Count()
        })
        .OrderByDescending(x => x.Students)
        .ToList();

        

        // ---------------------------------------------------------
        // Scheduled Classes
        // ---------------------------------------------------------

       var scheduledClasses = await _context.ScheduledClasses
    .AsNoTracking()
    .Where(x =>
        !x.IsDeleted &&
        filteredStudentIds.Contains(
            x.TeachingSchedule.Enrollment.StudentId))
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Course)
            .ToListAsync();

        // ---------------------------------------------------------
        // Attendance
        // ---------------------------------------------------------

        var attendance = await _context.Attendances
            .AsNoTracking()
            .Where(x =>
    !x.IsDeleted &&
    filteredStudentIds.Contains(
        x.ScheduledClass.TeachingSchedule.Enrollment.StudentId))
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Student)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Course)
            .ToListAsync();

        var attendanceTotal = attendance.Count;

        if (attendanceTotal > 0)
        {
            response.AverageAttendance =
                Math.Round(
                    attendance.Count(x =>
                        x.Status == AttendanceStatus.Present ||
                        x.Status == AttendanceStatus.Late)
                    * 100m / attendanceTotal,
                    2);

            response.AttendanceSummary = new AttendanceSummaryDashboardDto
            {
                PresentPercentage =
                    Math.Round(
                        attendance.Count(
                            x => x.Status == AttendanceStatus.Present)
                        * 100m / attendanceTotal,
                        2),

                AbsentPercentage =
                    Math.Round(
                        attendance.Count(
                            x => x.Status == AttendanceStatus.Absent)
                        * 100m / attendanceTotal,
                        2),

                LeavePercentage =
                    Math.Round(
                        attendance.Count(
                            x => x.Status == AttendanceStatus.Leave)
                        * 100m / attendanceTotal,
                        2),

                LatePercentage =
                    Math.Round(
                        attendance.Count(
                            x => x.Status == AttendanceStatus.Late)
                        * 100m / attendanceTotal,
                        2)
            };
        }

        // Monthly attendance trend
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

                return new StudentAttendanceTrendDto
                {
                    Month = new DateTime(
                        g.Key.Year,
                        g.Key.Month,
                        1).ToString("MMM yyyy"),

                    Attendance = total == 0
                        ? 0
                        : Math.Round(
                            g.Count(x =>
                                x.Status == AttendanceStatus.Present ||
                                x.Status == AttendanceStatus.Late)
                            * 100m / total,
                            2)
                };
            })
            .ToList();

        // ---------------------------------------------------------
        // Class Reports / Performance
        // ---------------------------------------------------------

        var classReports = await _context.ClassReports
            .AsNoTracking()
            .Where(x =>
    !x.IsDeleted &&
    filteredStudentIds.Contains(
        x.ScheduledClass.TeachingSchedule.Enrollment.StudentId))
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Student)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Course)
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
                .Select(g => new StudentPerformanceTrendDto
                {
                    Month = new DateTime(
                        g.Key.Year,
                        g.Key.Month,
                        1).ToString("MMM yyyy"),

                    Score = Math.Round(
    g.Average(
        x => (decimal)x.PerformanceRating * 20m),
    2)
                })
                .ToList();

            // Student ranking
            var rankingData = classReports
                .Where(x =>
                    x.ScheduledClass.TeachingSchedule.Enrollment.Student != null &&
                    x.ScheduledClass.TeachingSchedule.Enrollment.Course != null)
                .GroupBy(x =>
                    new
                    {
                        StudentId =
                            x.ScheduledClass.TeachingSchedule.Enrollment.StudentId,

                        StudentName =
                            x.ScheduledClass.TeachingSchedule.Enrollment.Student.FullName,

                        CourseName =
                            x.ScheduledClass.TeachingSchedule.Enrollment.Course.CourseName
                    })
                .Select(g =>
                {
                    var studentAttendance = attendance
                        .Where(a =>
                            a.ScheduledClass.TeachingSchedule.Enrollment.StudentId
                            == g.Key.StudentId)
                        .ToList();

                    var attendancePercentage =
                        studentAttendance.Count == 0
                            ? 0
                            : Math.Round(
                                studentAttendance.Count(a =>
                                    a.Status == AttendanceStatus.Present ||
                                    a.Status == AttendanceStatus.Late)
                                * 100m / studentAttendance.Count,
                                2);

                    return new StudentRankingDto
                    {
                        Rank = 0,
                        StudentName = g.Key.StudentName,
                        CourseName = g.Key.CourseName,
                        PerformanceScore = Math.Round(
    g.Average(
        x => (decimal)x.PerformanceRating * 20m),
    2),
                        Attendance = attendancePercentage
                    };
                })
                .OrderByDescending(x => x.PerformanceScore)
                .ThenByDescending(x => x.Attendance)
                .Take(10)
                .ToList();

            for (var i = 0; i < rankingData.Count; i++)
            {
                rankingData[i].Rank = i + 1;
            }

            response.StudentRanking = rankingData;

            // Best student
            var best = response.StudentRanking.FirstOrDefault();

            if (best != null)
            {
                response.BestStudent = new BestStudentDto
                {
                    StudentName = best.StudentName,
                    CourseName = best.CourseName,
                    PerformanceScore = best.PerformanceScore,
                    AttendancePercentage = best.Attendance
                };
            }
        }

        // ---------------------------------------------------------
        // Class Progress / Learning
        // ---------------------------------------------------------

        var classProgresses = await _context.ClassProgresses
            .AsNoTracking()
           .Where(x =>
    !x.IsDeleted &&
    filteredStudentIds.Contains(
        x.ScheduledClass.TeachingSchedule.Enrollment.StudentId))
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Student)
            .Include(x => x.ScheduledClass)
                .ThenInclude(x => x.TeachingSchedule)
                    .ThenInclude(x => x.Enrollment)
                        .ThenInclude(x => x.Course)
            .ToListAsync();

        if (classProgresses.Count > 0)
        {
            response.LearningSummary = new StudentLearningSummaryDto
            {
                AverageProgress =
                    Math.Round(
                        classProgresses.Average(
                            x => x.IsLessonCompleted ? 100m : 0m),
                        2),

                TotalLessonsCompleted =
                    classProgresses.Count(
                        x => x.IsLessonCompleted)
            };

            response.LearningProgress = classProgresses
                .Where(x =>
                    x.ScheduledClass.TeachingSchedule.Enrollment.Student != null &&
                    x.ScheduledClass.TeachingSchedule.Enrollment.Course != null)
                .GroupBy(x =>
                    new
                    {
                        StudentId =
                            x.ScheduledClass.TeachingSchedule.Enrollment.StudentId,

                        StudentName =
                            x.ScheduledClass.TeachingSchedule.Enrollment.Student.FullName,

                        CourseName =
                            x.ScheduledClass.TeachingSchedule.Enrollment.Course.CourseName
                    })
                .Select(g =>
                {
                    var totalLessons = g.Count();
                    var completedLessons =
                        g.Count(x => x.IsLessonCompleted);

                    var pagesCovered = g
                        .Select(x => x.PagesCovered)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Sum(ParsePagesCovered);

                    return new StudentLearningProgressDto
                    {
                        StudentName = g.Key.StudentName,
                        CourseName = g.Key.CourseName,
                        LessonsCompleted = completedLessons,
                        PagesCovered = pagesCovered,
                        ProgressPercentage = totalLessons == 0
                            ? 0
                            : Math.Round(
                                completedLessons * 100m / totalLessons,
                                2)
                    };
                })
                .OrderByDescending(x => x.ProgressPercentage)
                .ToList();
        }

        // ---------------------------------------------------------
        // Fee Summary / Fee Status
        // ---------------------------------------------------------

        var invoices = await _context.FeeInvoices
            .AsNoTracking()
            .Where(x =>
    !x.IsDeleted &&
    filteredStudentIds.Contains(x.Enrollment.StudentId))
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .ToListAsync();

       var payments = await _context.FeePayments
    .AsNoTracking()
    .Where(x => !x.IsDeleted)
    .Where(x =>
        invoices.Select(i => i.Id)
            .Contains(x.FeeInvoiceId))
    .ToListAsync();

        response.FeeSummary = new StudentFeeSummaryDto
        {
            TotalCollected = payments.Sum(x => x.Amount),

            PendingAmount = invoices
                .Where(x => x.BalanceAmount > 0)
                .Sum(x => x.BalanceAmount),

            PaidStudents = invoices
                .Where(x => x.BalanceAmount <= 0)
                .Select(x => x.Enrollment.StudentId)
                .Distinct()
                .Count(),

            PendingStudents = invoices
                .Where(x => x.BalanceAmount > 0)
                .Select(x => x.Enrollment.StudentId)
                .Distinct()
                .Count()
        };

        response.FeeStatus = invoices
            .Where(x =>
                x.Enrollment.Student != null &&
                x.Enrollment.Course != null)
            .GroupBy(x => x.Enrollment.StudentId)
            .Select(g =>
            {
                var latestInvoice = g
                    .OrderByDescending(x => x.InvoiceDate)
                    .First();

                var lastPaymentDate = g
                    .Where(x => x.PaidDate.HasValue)
                    .OrderByDescending(x => x.PaidDate)
                    .Select(x => x.PaidDate)
                    .FirstOrDefault();

                return new StudentFeeStatusDto
                {
                    StudentName =
                        latestInvoice.Enrollment.Student.FullName,

                    CourseName =
                        latestInvoice.Enrollment.Course.CourseName,

                    MonthlyFee =
                        latestInvoice.Enrollment.FinalMonthlyFee,

                    LastPaymentDate =
                        lastPaymentDate.HasValue
                            ? DateOnly.FromDateTime(lastPaymentDate.Value)
                            : default,

                    NextDueDate =
                        DateOnly.FromDateTime(
                            latestInvoice.DueDate),

                    Status =
                        latestInvoice.BalanceAmount <= 0
                            ? "Paid"
                            : "Pending"
                };
            })
            .OrderBy(x => x.NextDueDate)
            .ToList();

        return response;
    }

    private static int ParsePagesCovered(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0;

        var numbers = value
            .Split(
                new[] { ',', ';', '-', ' ' },
                StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                return int.TryParse(x, out var number)
                    ? number
                    : 0;
            })
            .Where(x => x > 0)
            .ToList();

        return numbers.Count == 0
            ? 0
            : numbers.Max();
    }
}