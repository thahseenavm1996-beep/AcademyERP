using AcademyERP.Application.DTOs.ReportUnlockRequests;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.ReportUnlockRequests;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class ReportUnlockRequestService : IReportUnlockRequestService
{
    private readonly ApplicationDbContext _context;

    public ReportUnlockRequestService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReportUnlockRequestResponse> CreateAsync(
        Guid applicationUserId,
        CreateReportUnlockRequest request)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(x =>
                x.ApplicationUserId == applicationUserId &&
                !x.IsDeleted);

        if (teacher == null)
            throw new InvalidOperationException("Teacher profile not found.");

        var scheduledClass = await _context.ScheduledClasses
            .Include(x => x.TeachingSchedule)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .FirstOrDefaultAsync(x =>
                x.Id == request.ScheduledClassId &&
                !x.IsDeleted);

        if (scheduledClass == null)
            throw new InvalidOperationException("Scheduled class not found.");

        if (scheduledClass.TeachingSchedule.TeacherId != teacher.Id)
            throw new UnauthorizedAccessException(
                "You are not authorized to request an unlock for this class.");

      var hasUnlockableClasses =
    await _context.ScheduledClasses
        .Where(x =>
            x.TeachingSchedule.TeacherId == teacher.Id &&
            x.ClassDate == scheduledClass.ClassDate &&
            !x.IsDeleted &&
            x.Status != ScheduledClassStatus.Cancelled)
        .AnyAsync(x =>
            !_context.ClassReports.Any(r =>
                r.ScheduledClassId == x.Id &&
                !r.IsDeleted));

if (!hasUnlockableClasses)
{
    throw new InvalidOperationException(
        "There are no class reports requiring an unlock for this date.");
}

var pendingRequest =
    await _context.ReportUnlockRequests
        .AnyAsync(x =>
            x.TeacherId == teacher.Id &&
            x.ReportDate == scheduledClass.ClassDate &&
            x.Status == ReportUnlockRequestStatus.Pending &&
            !x.IsDeleted);

if (pendingRequest)
{
    throw new InvalidOperationException(
        "An unlock request is already pending for this date.");
}

        if (string.IsNullOrWhiteSpace(request.Reason))
            throw new InvalidOperationException(
                "Please provide a reason for the unlock request.");

        var entity = new ReportUnlockRequest
        {
            ScheduledClassId = scheduledClass.Id,
            TeacherId = teacher.Id,
            ReportDate = scheduledClass.ClassDate,
            Reason = request.Reason.Trim(),
            Status = ReportUnlockRequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await _context.ReportUnlockRequests.AddAsync(entity);
        await _context.SaveChangesAsync();

        return await GetResponseAsync(entity.Id)
            ?? throw new InvalidOperationException(
                "Unable to create unlock request.");
    }

    public async Task<List<ReportUnlockRequestResponse>> GetForTeacherAsync(
        Guid applicationUserId)
    {
        var teacherId = await _context.Teachers
            .Where(x =>
                x.ApplicationUserId == applicationUserId &&
                !x.IsDeleted)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if (teacherId == Guid.Empty)
            return new();

        return await BuildQuery()
            .Where(x => x.TeacherId == teacherId)
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();
    }

    public async Task<List<ReportUnlockRequestResponse>> GetAllAsync(
        ReportUnlockRequestStatus? status = null)
    {
        var query = BuildQuery();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query
            .OrderByDescending(x => x.RequestedAt)
            .ToListAsync();
    }

    public async Task<bool> ReviewAsync(
        Guid requestId,
        Guid reviewedByUserId,
        ReportUnlockRequestStatus status,
        string? adminRemarks)
    {
        if (status != ReportUnlockRequestStatus.Approved &&
            status != ReportUnlockRequestStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Only Approved or Rejected is allowed.");
        }

        var request = await _context.ReportUnlockRequests
            .FirstOrDefaultAsync(x =>
                x.Id == requestId &&
                !x.IsDeleted);

        if (request == null)
            return false;

        if (request.Status != ReportUnlockRequestStatus.Pending)
            throw new InvalidOperationException(
                "This unlock request has already been reviewed.");

        request.Status = status;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedByUserId = reviewedByUserId;
        request.AdminRemarks = string.IsNullOrWhiteSpace(adminRemarks)
            ? null
            : adminRemarks.Trim();

        await _context.SaveChangesAsync();

        return true;
    }

    private IQueryable<ReportUnlockRequestResponse> BuildQuery()
    {
        return _context.ReportUnlockRequests
            .Where(x => !x.IsDeleted)
            .Select(x => new ReportUnlockRequestResponse
            {
                Id = x.Id,
                ScheduledClassId = x.ScheduledClassId,
                TeacherId = x.TeacherId,
                TeacherName = x.Teacher.FullName,
                StudentName =
    x.ScheduledClass.TeachingSchedule.Enrollment.Student.FullName,

CourseName =
    x.ScheduledClass.TeachingSchedule.Enrollment.Course.CourseName,

StudentCount =
    _context.ScheduledClasses
        .Where(sc =>
            sc.TeachingSchedule.TeacherId == x.TeacherId &&
            sc.ClassDate == x.ReportDate &&
            !sc.IsDeleted)
        .Select(sc =>
            sc.TeachingSchedule.Enrollment.StudentId)
        .Distinct()
        .Count(),

CourseCount =
    _context.ScheduledClasses
        .Where(sc =>
            sc.TeachingSchedule.TeacherId == x.TeacherId &&
            sc.ClassDate == x.ReportDate &&
            !sc.IsDeleted)
        .Select(sc =>
            sc.TeachingSchedule.Enrollment.CourseId)
        .Distinct()
        .Count(),

ReportDate = x.ReportDate,
                Reason = x.Reason,
                Status = x.Status,
                RequestedAt = x.RequestedAt,
                ReviewedAt = x.ReviewedAt,
                ReviewedByUserId = x.ReviewedByUserId,
                AdminRemarks = x.AdminRemarks
            });
    }

    private async Task<ReportUnlockRequestResponse?> GetResponseAsync(
        Guid id)
    {
        return await BuildQuery()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
