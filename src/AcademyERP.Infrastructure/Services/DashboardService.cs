using AcademyERP.Application.DTOs.Dashboard;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;

    public DashboardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryResponse> GetSummaryAsync()
    {

        var today = DateOnly.FromDateTime(DateTime.Now);

var todayClassesQuery = _context.ScheduledClasses
    .Where(x =>
        !x.IsDeleted &&
        x.ClassDate == today);
        var response = new DashboardSummaryResponse
        {
            TotalStudents = await _context.Students
                .CountAsync(x => !x.IsDeleted),

            TotalTeachers = await _context.Teachers
                .CountAsync(x => !x.IsDeleted),

            TotalParents = await _context.Parents
                .CountAsync(x => !x.IsDeleted),

            TotalPrograms = await _context.Programs
                .CountAsync(x => !x.IsDeleted),

            NewAdmissions = await _context.AdmissionApplications
                .CountAsync(x => !x.IsDeleted && x.ViewedByAdminAt == null),

            NewRegistrations = await _context.RegistrationRequests
                .CountAsync(x => !x.IsDeleted && x.ViewedByAdminAt == null),

            ActiveStudents = await _context.Students
                .CountAsync(x =>
                    !x.IsDeleted &&
                    x.Status == UserStatus.Active),

            ActiveTeachers = await _context.Teachers
                .CountAsync(x =>
                    !x.IsDeleted &&
                    x.Status == UserStatus.Active),

            // Finance
            TotalCollected = await _context.FeePayments
                .Where(x => !x.IsDeleted)
                .SumAsync(x => (decimal?)x.Amount) ?? 0m,

            TotalInvoiced = await _context.FeeInvoices
                .Where(x => !x.IsDeleted)
                .SumAsync(x => (decimal?)x.Amount) ?? 0m,

            PendingFees = await _context.FeeInvoices
                .Where(x =>
                    !x.IsDeleted &&
                    x.BalanceAmount > 0)
                .SumAsync(x => (decimal?)x.BalanceAmount) ?? 0m,

            OverdueFees = await _context.FeeInvoices
                .Where(x =>
                    !x.IsDeleted &&
                    x.BalanceAmount > 0 &&
                    x.DueDate < DateTime.UtcNow)
                .SumAsync(x => (decimal?)x.BalanceAmount) ?? 0m,


                TodayClasses = await todayClassesQuery
    .CountAsync(),

TodayCompletedClasses = await todayClassesQuery
    .CountAsync(x =>
        x.Status == ScheduledClassStatus.Completed),

TodayUpcomingClasses = await todayClassesQuery
    .CountAsync(x =>
        x.Status == ScheduledClassStatus.Scheduled),

TodayMissedClasses = await todayClassesQuery
    .CountAsync(x =>
        x.Status == ScheduledClassStatus.Missed),

TodayTeachers = await todayClassesQuery
    .Select(x =>
        x.TeachingSchedule.TeacherId)
    .Distinct()
    .CountAsync(),

TodayStudents = await todayClassesQuery
    .Select(x =>
        x.TeachingSchedule.Enrollment.StudentId)
    .Distinct()
    .CountAsync()


        };

        response.RecentStudents = await _context.Students
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
            .Select(x => new RecentStudentDto
            {
                Id = x.Id,
                AdmissionNumber = x.AdmissionNumber,
                FullName = x.FullName,
                Country = x.Country
            })
            .ToListAsync();
var recentActivities = new List<RecentActivityDto>();

var recentStudents = await _context.Students
    .Where(x => !x.IsDeleted)
    .OrderByDescending(x => x.CreatedAt)
    .Take(5)
    .Select(x => new RecentActivityDto
    {
        ActivityType = "Student",
        Description = $"New student registered: {x.FullName}",
        OccurredAt = x.CreatedAt
    })
    .ToListAsync();

var recentEnrollments = await _context.Enrollments
    .Where(x => !x.IsDeleted)
    .Include(x => x.Student)
    .OrderByDescending(x => x.CreatedAt)
    .Take(5)
    .Select(x => new RecentActivityDto
{
    EntityId = x.Id,
    ActivityType = "Enrollment",
    Description = $"New enrollment created for {x.Student.FullName}",
    OccurredAt = x.CreatedAt
})
    .ToListAsync();

var recentInvoices = await _context.FeeInvoices
    .Where(x => !x.IsDeleted)
    .Include(x => x.Enrollment)
        .ThenInclude(x => x.Student)
    .OrderByDescending(x => x.CreatedAt)
    .Take(5)
    .Select(x => new RecentActivityDto
{
    EntityId = x.Id,
    ActivityType = "Invoice",
    Description = $"Fee invoice generated for {x.Enrollment.Student.FullName}",
    OccurredAt = x.CreatedAt
})
    .ToListAsync();

var recentPayments = await _context.FeePayments
    .Where(x => !x.IsDeleted)
    .Include(x => x.FeeInvoice)
        .ThenInclude(x => x.Enrollment)
            .ThenInclude(x => x.Student)
    .OrderByDescending(x => x.CreatedAt)
    .Take(5)
    .Select(x => new RecentActivityDto
{
    EntityId = x.Id,
    ActivityType = "Payment",
    Description = $"Fee payment received from {x.FeeInvoice.Enrollment.Student.FullName}: ₹{x.Amount:N2}",
    OccurredAt = x.CreatedAt
})
    .ToListAsync();

var recentClasses = await _context.ScheduledClasses
    .Where(x => !x.IsDeleted)
    .Include(x => x.TeachingSchedule)
        .ThenInclude(x => x.Enrollment)
            .ThenInclude(x => x.Student)
    .OrderByDescending(x => x.CreatedAt)
    .Take(5)
    .Select(x => new RecentActivityDto
    {
        ActivityType = "Class",
        Description = $"Class scheduled for {x.TeachingSchedule.Enrollment.Student.FullName}",
        OccurredAt = x.CreatedAt
    })
    .ToListAsync();

var recentMissedClasses = await _context.ScheduledClasses
    .Where(x =>
        !x.IsDeleted &&
        x.Status == ScheduledClassStatus.Missed)
    .Include(x => x.TeachingSchedule)
        .ThenInclude(x => x.Enrollment)
            .ThenInclude(x => x.Student)
    .OrderByDescending(x => x.UpdatedAt)
    .Take(5)
    .Select(x => new RecentActivityDto
    {
        ActivityType = "Missed",
        Description = $"Student missed a class: {x.TeachingSchedule.Enrollment.Student.FullName}",
        OccurredAt = x.UpdatedAt ?? x.CreatedAt
    })
    .ToListAsync();
var recentRegistrations = await _context.RegistrationRequests
    .Where(x => !x.IsDeleted)
    .OrderByDescending(x => x.SubmittedAt)
    .Take(5)
    .Select(x => new RecentActivityDto
    {
        EntityId = x.Id,
        ActivityType = "Registration",
        Description = $"New registration request from {x.ParentName}",
        OccurredAt = x.SubmittedAt
    })
    .ToListAsync();
recentActivities.AddRange(recentStudents);
recentActivities.AddRange(recentEnrollments);
recentActivities.AddRange(recentInvoices);
recentActivities.AddRange(recentPayments);
recentActivities.AddRange(recentRegistrations);
recentActivities.AddRange(recentClasses);
recentActivities.AddRange(recentMissedClasses);

response.RecentActivities = recentActivities
    .OrderByDescending(x => x.OccurredAt)
    .GroupBy(x => new
    {
        x.ActivityType,
        x.Description
    })
    .Select(x => x.First())
    .Take(10)
    .ToList();


    var programPerformance = await _context.Programs
    .Where(p => !p.IsDeleted)
    .Select(p => new ProgramPerformanceDto
    {
        ProgramId = p.Id,
        ProgramName = p.ProgramName,

        EnrollmentCount = p.Courses
            .SelectMany(c => c.Enrollments)
            .Count(e => !e.IsDeleted),

        AttendanceCount = _context.Attendances
            .Count(a =>
                !a.IsDeleted &&
                a.ScheduledClass.TeachingSchedule.Enrollment.Course.ProgramId == p.Id),

        PresentCount = _context.Attendances
            .Count(a =>
                !a.IsDeleted &&
                a.ScheduledClass.TeachingSchedule.Enrollment.Course.ProgramId == p.Id &&
                (a.Status == AttendanceStatus.Present ||
                 a.Status == AttendanceStatus.Late)),

        PerformanceReportCount = _context.ClassReports
            .Count(r =>
                !r.IsDeleted &&
                r.ScheduledClass.TeachingSchedule.Enrollment.Course.ProgramId == p.Id),

        AveragePerformance = _context.ClassReports
            .Where(r =>
                !r.IsDeleted &&
                r.ScheduledClass.TeachingSchedule.Enrollment.Course.ProgramId == p.Id)
            .Select(r => (decimal?)r.PerformanceRating)
            .Average() ?? 0m
    })
    .OrderByDescending(x => x.EnrollmentCount)
    .ToListAsync();

foreach (var program in programPerformance)
{
    program.AttendancePercentage =
        program.AttendanceCount == 0
            ? 0
            : Math.Round(
                (decimal)program.PresentCount /
                program.AttendanceCount *
                100m,
                2);

    program.AveragePerformance =
        Math.Round(program.AveragePerformance, 2);
}

response.ProgramPerformance = programPerformance;

var mostEnrolledProgram = programPerformance
    .FirstOrDefault();

if (mostEnrolledProgram != null)
{
    response.MostEnrolledProgramEnrollments =
        await _context.Enrollments
            .Where(x =>
                !x.IsDeleted &&
                x.Course.ProgramId == mostEnrolledProgram.ProgramId)
            .OrderBy(x => x.Student.FullName)
            .Select(x => new DashboardEnrollmentDto
            {
                Id = x.Id,
                StudentName = x.Student.FullName,
                CourseName = x.Course.CourseName,
                TeacherName = x.Teacher != null
                    ? x.Teacher.FullName
                    : "Not Assigned",
                TimeSlotName = x.EnrollmentSchedules
    .Select(s => s.TimeSlot.Name)
    .FirstOrDefault() ?? "Not Assigned",

ClassDurationName = x.EnrollmentSchedules
    .Select(s => s.ClassDuration.Name)
    .FirstOrDefault() ?? "Not Assigned"
            })
            .ToListAsync();
}
response.TodayTeacherList =
    await todayClassesQuery
        .Select(x => new TodayTeacherDto
        {
            Id = x.TeachingSchedule.TeacherId,
            EmployeeCode = x.TeachingSchedule.Teacher.EmployeeCode,
            FullName = x.TeachingSchedule.Teacher.FullName
        })
        .Distinct()
        .OrderBy(x => x.FullName)
        .ToListAsync();

response.TodayStudentList =
    await todayClassesQuery
        .Select(x => new TodayStudentDto
        {
            Id = x.TeachingSchedule.Enrollment.StudentId,
            AdmissionNumber =
                x.TeachingSchedule.Enrollment.Student.AdmissionNumber,
            FullName =
                x.TeachingSchedule.Enrollment.Student.FullName
        })
        .Distinct()
        .OrderBy(x => x.FullName)
        .ToListAsync();

        response.RecentEnrollmentDetails =
    await _context.Enrollments
        .Where(x => !x.IsDeleted)
        .Include(x => x.Student)
        .Include(x => x.Course)
        .Include(x => x.Teacher)
        .Include(x => x.EnrollmentSchedules)
            .ThenInclude(x => x.TimeSlot)
        .Include(x => x.EnrollmentSchedules)
            .ThenInclude(x => x.ClassDuration)
        .OrderByDescending(x => x.CreatedAt)
        .Take(5)
        .Select(x => new DashboardEnrollmentDetailDto
        {
            Id = x.Id,
            StudentName = x.Student.FullName,
            CourseName = x.Course.CourseName,
            TeacherName = x.Teacher != null
                ? x.Teacher.FullName
                : "Not Assigned",
            TimeSlotName = x.EnrollmentSchedules
                .Select(s => s.TimeSlot.Name)
                .FirstOrDefault() ?? "Not Assigned",
            ClassDurationName = x.EnrollmentSchedules
                .Select(s => s.ClassDuration.Name)
                .FirstOrDefault() ?? "Not Assigned",
            StartDate = x.StartDate,
            MonthlyFee = x.MonthlyFee,
            FinalMonthlyFee = x.FinalMonthlyFee,
            NextBillingDate = x.NextBillingDate,
            Status = x.Status.ToString()
        })
        .ToListAsync();
        response.RecentFeeDetails =
    await _context.FeeInvoices
        .Where(x => !x.IsDeleted)
        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Student)
        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Course)
        .OrderByDescending(x => x.CreatedAt)
        .Take(5)
        .Select(x => new DashboardFeeDetailDto
        {
            Id = x.Id,
            InvoiceNumber = x.InvoiceNumber,
            StudentName = x.Enrollment.Student.FullName,
            CourseName = x.Enrollment.Course.CourseName,
            Amount = x.Amount,
            DiscountAmount = x.DiscountAmount,
            PaidAmount = x.PaidAmount,
            BalanceAmount = x.BalanceAmount,
            InvoiceDate = x.InvoiceDate,
            DueDate = x.DueDate,
            Status = x.Status.ToString()
        })
        .ToListAsync();

        response.RecentRegistrationDetails =
    await _context.RegistrationRequests
        .Where(x => !x.IsDeleted)
        .OrderByDescending(x => x.SubmittedAt)
        .Take(5)
        .Select(x => new DashboardRegistrationDetailDto
        {
            Id = x.Id,
            ParentName = x.ParentName,
            Email = x.Email,
            PhoneNumber = x.PhoneNumber,
            Country = x.Country,
            StudentNames = string.Join(
                ", ",
                x.Students.Select(s => s.StudentName)),
            StudentCount = x.Students.Count,
            Status = x.Status.ToString(),
            SubmittedAt = x.SubmittedAt
        })
        .ToListAsync();
        return response;
    }
}