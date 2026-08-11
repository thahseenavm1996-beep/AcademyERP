using AcademyERP.Application.Common;
using AcademyERP.Application.DTOs.Fees;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.Finance;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AcademyERP.Infrastructure.Services;

public class FeeInvoiceService : IFeeInvoiceService
{
    private readonly ApplicationDbContext _context;

    public FeeInvoiceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<FeeInvoiceResponse>> GetAllAsync(
        FeeInvoiceQueryRequest request)
    {
        var query = _context.FeeInvoices
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .AsQueryable();

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.StudentId.HasValue)
        {
            query = query.Where(x =>
                x.Enrollment.StudentId == request.StudentId.Value);
        }

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

    public async Task<FeeInvoiceResponse?> GetByIdAsync(Guid id)
    {
        var invoice = await _context.FeeInvoices
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
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
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
            return null;

        invoice.PaidAmount = request.PaidAmount;

        invoice.BalanceAmount =
            invoice.Amount - invoice.PaidAmount;

        invoice.Status = request.Status;

        invoice.PaidDate = request.PaidDate;

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

            CourseName =
                invoice.Enrollment.Course.CourseName,

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
}