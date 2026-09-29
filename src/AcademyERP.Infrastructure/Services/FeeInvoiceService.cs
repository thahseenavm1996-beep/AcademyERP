using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Fees;
using AcademyERP.Application.Interfaces;
using AcademyERP.Application.Services;
using AcademyERP.Application.DTOs.Enrollments;
using AcademyERP.Domain.Entities.Enrollments;
using AcademyERP.Domain.Entities.Finance;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class FeeInvoiceService : IFeeInvoiceService
{
    private readonly ApplicationDbContext _context;
    private readonly IEnrollmentService _enrollmentService;

    public FeeInvoiceService(
        ApplicationDbContext context,
        IEnrollmentService enrollmentService)
    {
        _context = context;
        _enrollmentService = enrollmentService;
    }

    public async Task<PagedResponse<FeeInvoiceResponse>> GetAllAsync(
        FeeInvoiceQueryRequest request)
    {
        var query = BuildFilteredQuery(request);

        var totalCount = await query.CountAsync();

        var invoices = await query
            .OrderByDescending(x => x.InvoiceDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<FeeInvoiceResponse>
        {
            Items = invoices.Select(MapToResponse).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                (double)totalCount / request.PageSize)
        };
    }

    public async Task<List<FeeInvoiceResponse>> GetAllForExportAsync(
        FeeInvoiceQueryRequest request)
    {
        var query = BuildFilteredQuery(request);

        var invoices = await query
            .OrderByDescending(x => x.InvoiceDate)
            .ToListAsync();

        return invoices
            .Select(MapToResponse)
            .ToList();
    }

    private IQueryable<FeeInvoice> BuildFilteredQuery(
        FeeInvoiceQueryRequest request)
    {
        var query = _context.FeeInvoices
            .Where(x => !x.IsDeleted)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
                    .ThenInclude(x => x.StudentParents)
                        .ThenInclude(x => x.Parent)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Teacher)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            var today = DateTime.Today;

            query = request.Status.Value switch
            {
                FeeStatus.Pending =>
                    query.Where(x =>
                        x.BalanceAmount > 0),

                FeeStatus.PartiallyPaid =>
                    query.Where(x =>
                        x.PaidAmount > 0 &&
                        x.BalanceAmount > 0),

                FeeStatus.Paid =>
                    query.Where(x =>
                        x.BalanceAmount <= 0 ||
                        x.Status == FeeStatus.Paid),

                FeeStatus.Overdue =>
                    query.Where(x =>
                        x.BalanceAmount > 0 &&
                        x.DueDate < today),

                _ => query
            };
        }

        if (request.StudentId.HasValue)
        {
            query = query.Where(x =>
                x.Enrollment.StudentId == request.StudentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.StudentName))
        {
            var studentName = request.StudentName.Trim();

            query = query.Where(x =>
                x.Enrollment.Student.FullName.Contains(studentName));
        }

        if (!string.IsNullOrWhiteSpace(request.TeacherName))
        {
            var teacherName = request.TeacherName.Trim();

            query = query.Where(x =>
                x.Enrollment.Teacher != null &&
                x.Enrollment.Teacher.FullName.Contains(teacherName));
        }

        if (request.TeacherId.HasValue)
        {
            query = query.Where(x =>
                x.Enrollment.TeacherId == request.TeacherId.Value);
        }

        if (request.CourseId.HasValue)
        {
            query = query.Where(x =>
                x.Enrollment.CourseId == request.CourseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Gender) &&
            !request.Gender.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(x =>
                x.Enrollment.Student.Gender.ToString() ==
                request.Gender);
        }

        return query;
    }

    public async Task<FeeInvoiceResponse?> GetByIdAsync(Guid id)
    {
        var invoice = await _context.FeeInvoices
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
                    .ThenInclude(x => x.StudentParents)
                        .ThenInclude(x => x.Parent)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            return null;

        return MapToResponse(invoice);
    }

    public async Task<FeeInvoiceResponse> CreateAsync(
        CreateFeeInvoiceRequest request)
    {
        var enrollment = await _context.Enrollments
            .Include(x => x.Student)
            .Include(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == request.EnrollmentId);

        if (enrollment == null)
            throw new Exception("Enrollment not found.");

        var amount = enrollment.FinalMonthlyFee;

        var invoice = new FeeInvoice
        {
            EnrollmentId = enrollment.Id,
            InvoiceNumber =
                $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",

            Amount = amount,
            DiscountAmount =
                enrollment.DiscountAmount +
                enrollment.ScholarshipAmount,

            PaidAmount = 0,
            BalanceAmount = amount,

            InvoiceDate = request.InvoiceDate,
            DueDate = request.DueDate,

            Status = FeeStatus.Pending,
            
            Remarks = request.Remarks
        };

        _context.FeeInvoices.Add(invoice);

        await _context.SaveChangesAsync();

        invoice = await _context.FeeInvoices
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
                    .ThenInclude(x => x.StudentParents)
                        .ThenInclude(x => x.Parent)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .FirstAsync(x => x.Id == invoice.Id);

        return MapToResponse(invoice);
    }

    public async Task<FeeInvoiceResponse?> UpdateAsync(
        Guid id,
        UpdateFeeInvoiceRequest request)
    {
        var invoice = await _context.FeeInvoices
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
                    .ThenInclude(x => x.StudentParents)
                        .ThenInclude(x => x.Parent)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            return null;

        invoice.Status = request.Status;

invoice.PaidDate = request.PaidDate;

invoice.DueDate = request.DueDate;

invoice.Remarks = request.Remarks;
        await _context.SaveChangesAsync();

        return MapToResponse(invoice);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invoice = await _context.FeeInvoices
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            return false;

        _context.FeeInvoices.Remove(invoice);

        await _context.SaveChangesAsync();

        return true;
    }

   public async Task<int> GeneratePendingCyclesAsync(DateTime asOfDate)
{
    var today = asOfDate.Date;

    var enrollments = await _context.Enrollments
        .Include(x => x.FeePauses)
        .Where(x => x.Status == EnrollmentStatus.Active &&
                    x.NextBillingDate.Date <= today)
        .ToListAsync();

    var generated = 0;

    foreach (var enrollment in enrollments)
    {
        while (enrollment.NextBillingDate.Date <= today)
        {
            var billingDate = enrollment.NextBillingDate.Date;

            if (enrollment.EndDate.HasValue &&
                enrollment.EndDate.Value.Date < billingDate)
            {
                break;
            }

            if (enrollment.FeePauses.Any(x =>
                    !x.ResumeDate.HasValue &&
                    x.PauseStartDate.Date <= billingDate))
            {
                break;
            }

            var alreadyGenerated = await _context.FeeInvoices
                .AnyAsync(x =>
                    x.EnrollmentId == enrollment.Id &&
                    x.InvoiceDate >= billingDate &&
                    x.InvoiceDate < billingDate.AddDays(1) &&
                    !x.IsDeleted);

            if (!alreadyGenerated)
            {
                var invoice = new FeeInvoice
                {
                    EnrollmentId = enrollment.Id,
                    InvoiceNumber =
                        $"INV-{billingDate:yyyyMMdd}-{enrollment.Id.ToString()[..8]}",
                    Amount = enrollment.FinalMonthlyFee,
                    DiscountAmount =
                        enrollment.DiscountAmount +
                        enrollment.ScholarshipAmount,
                    PaidAmount = 0,
                    BalanceAmount = enrollment.FinalMonthlyFee,
                    InvoiceDate = billingDate,
                    DueDate = billingDate,
                    Status = FeeStatus.Pending,
                    Remarks = "Generated from the enrollment billing cycle."
                };

                _context.FeeInvoices.Add(invoice);

                try
                {
                    await _context.SaveChangesAsync();

                    generated++;
                }
                catch (DbUpdateException ex) when (
                    ex.InnerException?.Message.Contains(
                        "IX_FeeInvoices_EnrollmentId_InvoiceDate",
                        StringComparison.OrdinalIgnoreCase) == true)
                {
                    // Another request generated this invoice first.
                    // The database unique index prevented the duplicate.
                    _context.Entry(invoice).State = EntityState.Detached;
                }
            }

            enrollment.NextBillingDate = billingDate.AddMonths(1);

            await _context.SaveChangesAsync();
        }
    }

    return generated;
}

    public async Task<EnrollmentResponse?> PauseEnrollmentAsync(
        Guid enrollmentId,
        DateTime pauseStartDate,
        string? reason)
    {
        var enrollment = await _context.Enrollments
            .Include(x => x.FeePauses)
            .FirstOrDefaultAsync(x => x.Id == enrollmentId);

        if (enrollment == null)
            return null;

        var date = pauseStartDate.Date;

        if (date < enrollment.StartDate.Date)
            throw new InvalidOperationException(
                "Leave date cannot be before the enrollment start date.");

        if (enrollment.EndDate.HasValue &&
            date > enrollment.EndDate.Value.Date)
            throw new InvalidOperationException(
                "Leave date cannot be after the enrollment end date.");

        if (enrollment.FeePauses.Any(x => !x.ResumeDate.HasValue))
            throw new InvalidOperationException(
                "Fee generation is already paused for this enrollment.");

        if (enrollment.FeePauses.Any(x =>
                date >= x.PauseStartDate.Date &&
                (!x.ResumeDate.HasValue || date < x.ResumeDate.Value.Date)))
            throw new InvalidOperationException(
                "The leave date overlaps an existing fee pause.");

        _context.EnrollmentFeePauses.Add(new EnrollmentFeePause
        {
            EnrollmentId = enrollment.Id,
            PauseStartDate = date,
            Reason = reason
        });

        await _context.SaveChangesAsync();
        return await _enrollmentService.GetByIdAsync(enrollmentId);
    }

    public async Task<EnrollmentResponse?> ResumeEnrollmentAsync(
        Guid enrollmentId,
        DateTime resumeDate)
    {
        var enrollment = await _context.Enrollments
            .Include(x => x.FeePauses)
            .FirstOrDefaultAsync(x => x.Id == enrollmentId);

        if (enrollment == null)
            return null;

        var activePause = enrollment.FeePauses
            .Where(x => !x.ResumeDate.HasValue)
            .OrderByDescending(x => x.PauseStartDate)
            .FirstOrDefault();

        if (activePause == null)
            throw new InvalidOperationException(
                "Fee generation is not currently paused for this enrollment.");

        var date = resumeDate.Date;
        if (date <= activePause.PauseStartDate.Date)
            throw new InvalidOperationException(
                "Resume date must be after the leave start date.");

        if (enrollment.EndDate.HasValue &&
            date > enrollment.EndDate.Value.Date)
            throw new InvalidOperationException(
                "Resume date cannot be after the enrollment end date.");

        activePause.ResumeDate = date;
        enrollment.NextBillingDate = enrollment.NextBillingDate.Date
            .AddDays((date - activePause.PauseStartDate.Date).Days);

        await _context.SaveChangesAsync();
        return await _enrollmentService.GetByIdAsync(enrollmentId);
    }

    private static FeeInvoiceResponse MapToResponse(
        FeeInvoice invoice)
    {
        return new FeeInvoiceResponse
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,

            EnrollmentId = invoice.EnrollmentId,

            StudentName =
                invoice.Enrollment.Student.FullName,

            AdmissionNumber =
                invoice.Enrollment.Student.AdmissionNumber,

            TeacherName =
                invoice.Enrollment.Teacher != null
                    ? invoice.Enrollment.Teacher.FullName
                    : null,

            CourseName =
                invoice.Enrollment.Course.CourseName,

            Gender = invoice.Enrollment.Student.Gender.ToString(),

            ParentName = invoice.Enrollment.Student.StudentParents
                .OrderByDescending(x => x.IsPrimaryContact)
                .Select(x => x.Parent.FullName)
                .FirstOrDefault(),

          ParentPhoneNumber = invoice.Enrollment.Student.PhoneNumber,

            Amount = invoice.Amount,
            DiscountAmount = invoice.DiscountAmount,
            PaidAmount = invoice.PaidAmount,
            BalanceAmount = invoice.BalanceAmount,

            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            PaidDate = invoice.PaidDate,

            Status = invoice.Status,
            
            Remarks = invoice.Remarks
        };
    }

    public async Task<List<FeeInvoiceResponse>>
    GetTeacherOutstandingInvoicesAsync(Guid applicationUserId)
{
    var teacherId = await _context.Teachers
        .Where(x => x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();

    if (teacherId == Guid.Empty)
        return new List<FeeInvoiceResponse>();

    var invoices = await _context.FeeInvoices
        .AsNoTracking()
        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Student)
                .ThenInclude(x => x.StudentParents)
                    .ThenInclude(x => x.Parent)
        .Include(x => x.Enrollment)
            .ThenInclude(x => x.Course)
        .Where(x =>
            _context.TeachingSchedules.Any(s =>
                s.EnrollmentId == x.EnrollmentId &&
                s.TeacherId == teacherId &&
                s.IsActive) &&
            x.BalanceAmount > 0)
        .OrderBy(x => x.DueDate)
        .ToListAsync();

    return invoices.Select(MapToResponse).ToList();
}
}
