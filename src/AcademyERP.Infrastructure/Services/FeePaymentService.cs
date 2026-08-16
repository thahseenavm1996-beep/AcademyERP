using AcademyERP.Application.DTOs.Payments;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.Finance;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Application.Common;

namespace AcademyERP.Infrastructure.Services;

public class FeePaymentService : IFeePaymentService
{
    private readonly ApplicationDbContext _context;

    public FeePaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<FeePaymentResponse>> GetAllAsync(
    FeePaymentQueryRequest request)
{
    var query = _context.FeePayments
        .Include(x => x.FeeInvoice)
            .ThenInclude(x => x.Enrollment)
                .ThenInclude(x => x.Student)
        .Include(x => x.PaymentMethod)
        .AsQueryable();

    if (request.FeeInvoiceId.HasValue)
    {
        query = query.Where(x =>
            x.FeeInvoiceId ==
            request.FeeInvoiceId.Value);
    }

    var totalCount = await query.CountAsync();

    var payments = await query
        .OrderByDescending(x => x.PaymentDate)
        .Skip((request.Page - 1) * request.PageSize)
        .Take(request.PageSize)
        .ToListAsync();

    return new PagedResponse<FeePaymentResponse>
    {
        Items = payments
            .Select(MapToResponse)
            .ToList(),

        Page = request.Page,
        PageSize = request.PageSize,
        TotalCount = totalCount,

        TotalPages = (int)Math.Ceiling(
            (double)totalCount /
            request.PageSize)
    };
}

    public async Task<FeePaymentResponse?> GetByIdAsync(Guid id)
    {
        var payment = await _context.FeePayments
            .Include(x => x.FeeInvoice)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .Include(x => x.PaymentMethod)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (payment == null)
            return null;

        return MapToResponse(payment);
    }

    public async Task<FeePaymentResponse> CreateAsync(
        CreateFeePaymentRequest request)
    {
        var invoice = await _context.FeeInvoices
            .FirstOrDefaultAsync(x =>
                x.Id == request.FeeInvoiceId);

        if (invoice == null)
            throw new Exception("Invoice not found.");

        var paymentMethod =
            await _context.PaymentMethods
                .FirstOrDefaultAsync(x =>
                    x.Id == request.PaymentMethodId);

        if (paymentMethod == null)
            throw new Exception("Payment method not found.");

        var payment = new FeePayment
        {
            FeeInvoiceId = request.FeeInvoiceId,
            Amount = request.Amount,
            PaymentDate = request.PaymentDate,
            PaymentMethodId = request.PaymentMethodId,
            TransactionReference =
                request.TransactionReference,
            Remarks = request.Remarks
        };

        _context.FeePayments.Add(payment);

        invoice.PaidAmount += request.Amount;

        invoice.BalanceAmount =
            invoice.Amount - invoice.PaidAmount;

        if (invoice.PaidAmount <= 0)
        {
            invoice.Status = FeeStatus.Pending;
        }
        else if (invoice.BalanceAmount > 0)
        {
            invoice.Status = FeeStatus.PartiallyPaid;
        }
        else
        {
            invoice.Status = FeeStatus.Paid;
            invoice.PaidDate = request.PaymentDate;
        }

        await _context.SaveChangesAsync();

        payment = await _context.FeePayments
            .Include(x => x.FeeInvoice)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .Include(x => x.PaymentMethod)
            .FirstAsync(x => x.Id == payment.Id);

        return MapToResponse(payment);
    }

    public async Task<FeePaymentResponse?> UpdateAsync(
        Guid id,
        UpdateFeePaymentRequest request)
    {
        var payment = await _context.FeePayments
            .Include(x => x.FeeInvoice)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (payment == null)
            return null;

        payment.Amount = request.Amount;
        payment.PaymentDate = request.PaymentDate;
        payment.PaymentMethodId =
            request.PaymentMethodId;
        payment.TransactionReference =
            request.TransactionReference;
        payment.Remarks = request.Remarks;

        await _context.SaveChangesAsync();

        payment = await _context.FeePayments
            .Include(x => x.FeeInvoice)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .Include(x => x.PaymentMethod)
            .FirstAsync(x => x.Id == payment.Id);

        return MapToResponse(payment);
    }

   public async Task<bool> DeleteAsync(Guid id)
{
    var payment = await _context.FeePayments
        .FirstOrDefaultAsync(x => x.Id == id);

    if (payment == null)
        return false;

    var invoiceId = payment.FeeInvoiceId;

    _context.FeePayments.Remove(payment);

    await _context.SaveChangesAsync();

    var invoice = await _context.FeeInvoices
        .FirstOrDefaultAsync(x => x.Id == invoiceId);

    if (invoice != null)
    {
        var totalPaid = await _context.FeePayments
            .Where(x => x.FeeInvoiceId == invoiceId)
            .SumAsync(x => x.Amount);

        invoice.PaidAmount = totalPaid;

        invoice.BalanceAmount =
            invoice.Amount - totalPaid;

        if (totalPaid <= 0)
        {
            invoice.Status = FeeStatus.Pending;
        }
        else if (invoice.BalanceAmount > 0)
        {
            invoice.Status = FeeStatus.PartiallyPaid;
        }
        else
        {
            invoice.Status = FeeStatus.Paid;
        }

        await _context.SaveChangesAsync();
    }

    return true;
}

    private static FeePaymentResponse MapToResponse(
        FeePayment payment)
    {
        return new FeePaymentResponse
        {
            Id = payment.Id,
            FeeInvoiceId = payment.FeeInvoiceId,

            InvoiceNumber =
                payment.FeeInvoice.InvoiceNumber,

            StudentName =
                payment.FeeInvoice
                    .Enrollment
                    .Student
                    .FullName,

            Amount = payment.Amount,

            PaymentDate = payment.PaymentDate,

            PaymentMethodId =
                payment.PaymentMethodId,

            PaymentMethodName =
                payment.PaymentMethod.Name,

            TransactionReference =
                payment.TransactionReference,

            Remarks = payment.Remarks
        };
    }
}