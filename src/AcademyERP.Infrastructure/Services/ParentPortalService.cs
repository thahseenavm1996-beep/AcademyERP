using AcademyERP.Application.DTOs.ParentPortal;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Entities.Identity;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class ParentPortalService : IParentPortalService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ParentPortalService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    private async Task<Domain.Entities.Parents.Parent?> GetParentAsync(
        Guid applicationUserId)
    {
        return await _context.Parents
            .Include(x => x.StudentParents)
                .ThenInclude(x => x.Student)
                    .ThenInclude(x => x.Enrollments)
                        .ThenInclude(x => x.Course)
            .Include(x => x.StudentParents)
                .ThenInclude(x => x.Student)
                    .ThenInclude(x => x.Enrollments)
                        .ThenInclude(x => x.Teacher)
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.ApplicationUserId == applicationUserId);
    }

    public async Task<ParentDashboardResponse?> GetDashboardAsync(
        Guid applicationUserId,
        string parentName)
    {
        var parent = await GetParentAsync(applicationUserId);

        if (parent == null)
            return null;

        var response = new ParentDashboardResponse
        {
            ParentName = parentName,
            ChildrenCount = parent.StudentParents.Count
        };

        foreach (var studentParent in parent.StudentParents)
        {
            var student = studentParent.Student;

            var enrollment = student.Enrollments
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Status == EnrollmentStatus.Active)
                .FirstOrDefault();

            if (enrollment == null)
                continue;

            var attendanceRecords = await _context.Attendances
                .Where(a =>
                    !a.IsDeleted &&
                    a.ScheduledClass.TeachingSchedule.EnrollmentId == enrollment.Id)
                .ToListAsync();

            var totalAttendance = attendanceRecords.Count;

            var presentCount = attendanceRecords.Count(x =>
                x.Status == AttendanceStatus.Present ||
                x.Status == AttendanceStatus.Late);

            var attendancePercentage = totalAttendance == 0
                ? 0
                : Math.Round(
                    (decimal)presentCount / totalAttendance * 100,
                    2);

            var pastScheduledClasses = await _context.ScheduledClasses
                .Where(x =>
                    !x.IsDeleted &&
                    x.ClassDate <= DateOnly.FromDateTime(DateTime.Today) &&
                    x.TeachingSchedule.EnrollmentId == enrollment.Id)
                .CountAsync();

            var classReports = await _context.ClassReports
                .Where(x =>
                    !x.IsDeleted &&
                    x.ScheduledClass.TeachingSchedule.EnrollmentId == enrollment.Id)
                .CountAsync();

            var progressPercentage = pastScheduledClasses == 0
                ? 0
                : Math.Min(
                    100,
                    Math.Round(
                        (decimal)classReports / pastScheduledClasses * 100,
                        2));

            response.Children.Add(
                new ParentChildSummaryDto
                {
                    StudentId = student.Id,
                    StudentName = student.FullName,
                    CourseName = enrollment.Course.CourseName,
                    TeacherName = enrollment.Teacher?.FullName ?? "Not Assigned",
                    AttendancePercentage = attendancePercentage,
                    ProgressPercentage = progressPercentage,
                    HasAttendanceRecords = totalAttendance > 0,
                    HasProgressRecords = classReports > 0
                });
        }

        var linkedStudentIds = parent.StudentParents
            .Select(x => x.StudentId)
            .ToList();

        response.PendingFeeAmount = await _context.FeeInvoices
            .Where(x =>
                !x.IsDeleted &&
                linkedStudentIds.Contains(x.Enrollment.StudentId) &&
                x.BalanceAmount > 0)
            .SumAsync(x => x.BalanceAmount);

        response.UpcomingClasses = await _context.ScheduledClasses
            .Where(x =>
                !x.IsDeleted &&
                x.ClassDate >= DateOnly.FromDateTime(DateTime.Today) &&
                linkedStudentIds.Contains(
                    x.TeachingSchedule.Enrollment.StudentId))
            .OrderBy(x => x.ClassDate)
            .ThenBy(x => x.StartTime)
            .Take(5)
            .Select(x => new UpcomingClassDto
            {
                ClassDate = x.ClassDate.ToString("dd MMM yyyy"),
                StudentName =
                    x.TeachingSchedule.Enrollment.Student.FullName,
                CourseName =
                    x.TeachingSchedule.Enrollment.Course.CourseName,
                TeacherName =
                    x.TeachingSchedule.Teacher.FullName,
                StartTime =
                    x.StartTime.ToString("hh:mm tt"),
                DurationMinutes =
                    (int)(x.EndTime.ToTimeSpan() -
                          x.StartTime.ToTimeSpan()).TotalMinutes
            })
            .ToListAsync();

        return response;
    }

    public async Task<List<ParentChildSummaryDto>?> GetChildrenAsync(
        Guid applicationUserId)
    {
        var parent = await GetParentAsync(applicationUserId);

        if (parent == null)
            return null;

        var result = new List<ParentChildSummaryDto>();

        foreach (var studentParent in parent.StudentParents)
        {
            var student = studentParent.Student;

            var enrollment = student.Enrollments
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Status == EnrollmentStatus.Active)
                .FirstOrDefault();

            if (enrollment == null)
                continue;

            var attendanceRecords = await _context.Attendances
                .Where(a =>
                    !a.IsDeleted &&
                    a.ScheduledClass.TeachingSchedule.EnrollmentId ==
                    enrollment.Id)
                .ToListAsync();

            var totalAttendance = attendanceRecords.Count;

            var presentCount = attendanceRecords.Count(x =>
                x.Status == AttendanceStatus.Present ||
                x.Status == AttendanceStatus.Late);

            var attendancePercentage = totalAttendance == 0
                ? 0
                : Math.Round(
                    (decimal)presentCount / totalAttendance * 100,
                    2);

            var pastClasses = await _context.ScheduledClasses
                .Where(x =>
                    !x.IsDeleted &&
                    x.ClassDate <= DateOnly.FromDateTime(DateTime.Today) &&
                    x.TeachingSchedule.EnrollmentId == enrollment.Id)
                .CountAsync();

            var reports = await _context.ClassReports
                .Where(x =>
                    !x.IsDeleted &&
                    x.ScheduledClass.TeachingSchedule.EnrollmentId ==
                    enrollment.Id)
                .CountAsync();

            var progressPercentage = pastClasses == 0
                ? 0
                : Math.Min(
                    100,
                    Math.Round(
                        (decimal)reports / pastClasses * 100,
                        2));

            result.Add(new ParentChildSummaryDto
            {
                StudentId = student.Id,
                StudentName = student.FullName,
                CourseName = enrollment.Course.CourseName,
                TeacherName = enrollment.Teacher?.FullName ?? "Not Assigned",
                AttendancePercentage = attendancePercentage,
                ProgressPercentage = progressPercentage,
                HasAttendanceRecords = totalAttendance > 0,
                HasProgressRecords = reports > 0
            });
        }

        return result;
    }

    public async Task<List<ParentScheduleDto>?> GetScheduleAsync(
        Guid applicationUserId,
        Guid? studentId = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        var parent = await GetParentAsync(applicationUserId);

        if (parent == null)
            return null;

        var studentIds = parent.StudentParents
            .Select(x => x.StudentId)
            .ToList();

        if (studentId.HasValue)
            studentIds = studentIds
                .Where(x => x == studentId.Value)
                .ToList();

        var from = fromDate ??
                   DateOnly.FromDateTime(DateTime.Today);

        var to = toDate ??
                 from.AddDays(30);

        var classes = await _context.ScheduledClasses
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
                x.ClassDate >= from &&
                x.ClassDate <= to &&
                studentIds.Contains(
                    x.TeachingSchedule.Enrollment.StudentId))
            .OrderBy(x => x.ClassDate)
            .ThenBy(x => x.StartTime)
            .ToListAsync();

        return classes.Select(x => new ParentScheduleDto
        {
            ScheduledClassId = x.Id,
            StudentId = x.TeachingSchedule.Enrollment.StudentId,
            StudentName =
                x.TeachingSchedule.Enrollment.Student.FullName,
            CourseName =
                x.TeachingSchedule.Enrollment.Course.CourseName,
            TeacherName =
                x.TeachingSchedule.Teacher.FullName,
            ClassDate =
                x.ClassDate.ToString("dd MMM yyyy"),
            StartTime =
                x.StartTime.ToString("hh:mm tt"),
            EndTime =
                x.EndTime.ToString("hh:mm tt"),
            DurationMinutes =
                (int)(x.EndTime.ToTimeSpan() -
                      x.StartTime.ToTimeSpan()).TotalMinutes
        }).ToList();
    }

    public async Task<ParentAttendanceResponse?> GetAttendanceAsync(
        Guid applicationUserId,
        Guid? studentId = null,
        AttendanceStatus? status = null,
        DateOnly? fromDate = null,
        DateOnly? toDate = null)
    {
        var parent = await GetParentAsync(applicationUserId);

        if (parent == null)
            return null;

        var studentIds = parent.StudentParents
            .Select(x => x.StudentId)
            .ToList();

        if (studentId.HasValue)
            studentIds = studentIds
                .Where(x => x == studentId.Value)
                .ToList();

        var query = _context.Attendances
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
                studentIds.Contains(
                    x.ScheduledClass.TeachingSchedule.Enrollment.StudentId));

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        if (fromDate.HasValue)
            query = query.Where(x =>
                x.ScheduledClass.ClassDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x =>
                x.ScheduledClass.ClassDate <= toDate.Value);

        var records = await query
            .OrderByDescending(x => x.ScheduledClass.ClassDate)
            .ThenByDescending(x => x.ScheduledClass.StartTime)
            .ToListAsync();

        var total = records.Count;

        var present = records.Count(x =>
            x.Status == AttendanceStatus.Present);

        var late = records.Count(x =>
            x.Status == AttendanceStatus.Late);

        var absent = records.Count(x =>
            x.Status == AttendanceStatus.Absent);

        var leave = records.Count(x =>
            x.Status == AttendanceStatus.Leave);

        var attendanceRate = total == 0
            ? 0
            : Math.Round(
                (decimal)(present + late) / total * 100,
                2);

        return new ParentAttendanceResponse
        {
            Summary = new ParentAttendanceSummaryDto
            {
                Total = total,
                Present = present,
                Absent = absent,
                Late = late,
                Leave = leave,
                AttendanceRate = attendanceRate
            },

            Records = records.Select(x => new ParentAttendanceDto
            {
                Id = x.Id,
                ScheduledClassId = x.ScheduledClassId,
                StudentId =
                    x.ScheduledClass.TeachingSchedule.Enrollment.StudentId,
                StudentName =
                    x.ScheduledClass.TeachingSchedule.Enrollment.Student.FullName,
                CourseName =
                    x.ScheduledClass.TeachingSchedule.Enrollment.Course.CourseName,
                TeacherName =
                    x.ScheduledClass.TeachingSchedule.Teacher.FullName,
                ClassDate =
                    x.ScheduledClass.ClassDate.ToString("dd MMM yyyy"),
                StartTime =
                    x.ScheduledClass.StartTime.ToString("hh:mm tt"),
                Status =
                    x.Status.ToString(),
                Remarks = x.Remarks
            }).ToList()
        };
    }

    public async Task<ParentFeesResponse?> GetFeesAsync(
        Guid applicationUserId,
        Guid? studentId = null)
    {
        var parent = await GetParentAsync(applicationUserId);

        if (parent == null)
            return null;

        var studentIds = parent.StudentParents
            .Select(x => x.StudentId)
            .ToList();

        if (studentId.HasValue)
            studentIds = studentIds
                .Where(x => x == studentId.Value)
                .ToList();

        var invoices = await _context.FeeInvoices
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .Where(x =>
                !x.IsDeleted &&
                studentIds.Contains(x.Enrollment.StudentId))
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync();

        var invoiceIds = invoices.Select(x => x.Id).ToList();

        var payments = await _context.FeePayments
            .Include(x => x.FeeInvoice)
            .Include(x => x.PaymentMethod)
            .Where(x =>
                !x.IsDeleted &&
                invoiceIds.Contains(x.FeeInvoiceId))
            .OrderByDescending(x => x.PaymentDate)
            .ToListAsync();

        var totalInvoiced = invoices.Sum(x => x.Amount);
        var totalPaid = payments.Sum(x => x.Amount);

        var positiveBalance = invoices
            .Where(x => x.BalanceAmount > 0)
            .Sum(x => x.BalanceAmount);

        var credit = invoices
            .Where(x => x.BalanceAmount < 0)
            .Sum(x => Math.Abs(x.BalanceAmount));

        return new ParentFeesResponse
        {
            TotalInvoiced = totalInvoiced,
            TotalPaid = totalPaid,
            OutstandingAmount = positiveBalance,
            CreditAmount = credit,

            Invoices = invoices.Select(x => new ParentFeeInvoiceDto
            {
                Id = x.Id,
                EnrollmentId = x.EnrollmentId,
                StudentName = x.Enrollment.Student.FullName,
                CourseName = x.Enrollment.Course.CourseName,
                InvoiceNumber = x.InvoiceNumber,
                Amount = x.Amount,
                DiscountAmount = x.DiscountAmount,
                PaidAmount = x.PaidAmount,
                BalanceAmount = x.BalanceAmount,
                InvoiceDate = x.InvoiceDate.ToString("dd MMM yyyy"),
                DueDate = x.DueDate.ToString("dd MMM yyyy"),
                PaidDate = x.PaidDate.HasValue
                    ? x.PaidDate.Value.ToString("dd MMM yyyy")
                    : null,
                Status = x.Status.ToString(),
                Remarks = x.Remarks
            }).ToList(),

            Payments = payments.Select(x => new ParentFeePaymentDto
            {
                Id = x.Id,
                InvoiceNumber = x.FeeInvoice.InvoiceNumber,
                StudentName = x.FeeInvoice.Enrollment.Student.FullName,
                Amount = x.Amount,
                PaymentDate = x.PaymentDate.ToString("dd MMM yyyy"),
                PaymentMethodName =
                    x.PaymentMethod?.Name ?? "Not Specified",
                TransactionReference =
                    x.TransactionReference,
                Remarks = x.Remarks
            }).ToList()
        };
    }

    public async Task<List<ParentProgressDto>?> GetProgressAsync(
        Guid applicationUserId,
        Guid? studentId = null)
    {
        var parent = await GetParentAsync(applicationUserId);

        if (parent == null)
            return null;

        var studentIds = parent.StudentParents
            .Select(x => x.StudentId)
            .ToList();

        if (studentId.HasValue)
            studentIds = studentIds
                .Where(x => x == studentId.Value)
                .ToList();

        var reports = await _context.ClassReports
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
                studentIds.Contains(
                    x.ScheduledClass.TeachingSchedule.Enrollment.StudentId))
            .OrderByDescending(x => x.ScheduledClass.ClassDate)
            .ToListAsync();

        var scheduledClassIds = reports
            .Select(x => x.ScheduledClassId)
            .ToList();

        var attendance = await _context.Attendances
            .Where(x =>
                !x.IsDeleted &&
                scheduledClassIds.Contains(x.ScheduledClassId))
            .ToListAsync();

        return reports.Select(x =>
        {
            var attendanceRecord = attendance
                .FirstOrDefault(a =>
                    a.ScheduledClassId == x.ScheduledClassId);

            return new ParentProgressDto
            {
                Id = x.Id,
                ScheduledClassId = x.ScheduledClassId,

                StudentId =
                    x.ScheduledClass.TeachingSchedule.Enrollment.StudentId,

                StudentName =
                    x.ScheduledClass.TeachingSchedule.Enrollment.Student.FullName,

                CourseName =
                    x.ScheduledClass.TeachingSchedule.Enrollment.Course.CourseName,

                TeacherName =
                    x.ScheduledClass.TeachingSchedule.Teacher.FullName,

                ClassDate =
                    x.ScheduledClass.ClassDate.ToString("dd MMM yyyy"),

                PerformanceRating = (int)x.PerformanceRating,
HomeworkStatus = x.HomeworkStatus.ToString(),
BehaviourRating = (int)x.BehaviourRating,
ClassOutcome = x.ClassOutcome.ToString(),
                LessonTaken = x.LessonTaken,
                NextHomework = x.NextHomework,
                TeacherRemarks = x.TeacherRemarks,
                ActualDurationMinutes = x.ActualDurationMinutes,
                SubmittedAt = x.SubmittedAt.ToString("dd MMM yyyy hh:mm tt"),

                AttendanceStatus =
                    attendanceRecord?.Status.ToString() ?? "Not Marked"
            };
        }).ToList();
    }

    public async Task<ParentProfileDto?> GetProfileAsync(
        Guid applicationUserId)
    {
        var parent = await _context.Parents
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.ApplicationUserId == applicationUserId);

        if (parent == null)
            return null;

        var user = await _userManager.FindByIdAsync(
            applicationUserId.ToString());

        return new ParentProfileDto
        {
            Id = parent.Id,
            ApplicationUserId = parent.ApplicationUserId,
            ParentNumber = parent.ParentNumber,
            FullName = parent.FullName,
            PhoneNumber = parent.PhoneNumber,
            Email = user?.Email ?? parent.Email,
            Country = parent.Country,
            TimeZone = parent.TimeZone,
            Address = parent.Address,
            Occupation = parent.Occupation,
            Remarks = parent.Remarks
        };
    }

    public async Task<bool> UpdateProfileAsync(
        Guid applicationUserId,
        UpdateParentProfileRequest request)
    {
        var parent = await _context.Parents
            .FirstOrDefaultAsync(x =>
                !x.IsDeleted &&
                x.ApplicationUserId == applicationUserId);

        if (parent == null)
            return false;

        var user = await _userManager.FindByIdAsync(
            applicationUserId.ToString());

        parent.FullName = request.FullName.Trim();
        parent.PhoneNumber = request.PhoneNumber.Trim();
        parent.Email = request.Email.Trim();
        parent.Country = request.Country.Trim();
        parent.TimeZone = request.TimeZone.Trim();
        parent.Address = request.Address.Trim();
        parent.Occupation = request.Occupation.Trim();
        parent.Remarks = request.Remarks;

        if (user != null)
        {
            user.FullName = request.FullName.Trim();
            user.Email = request.Email.Trim();
            user.UserName = request.Email.Trim();
            user.PhoneNumber = request.PhoneNumber.Trim();

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return false;
        }

        await _context.SaveChangesAsync();

        return true;
    }
}
