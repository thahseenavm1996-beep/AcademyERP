using AcademyERP.Application.DTOs.FinanceReports;
using AcademyERP.Application.Services;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AcademyERP.Domain.Entities.Finance;

namespace AcademyERP.Infrastructure.Services;

public class FinanceReportService : IFinanceReportService
{
    private readonly ApplicationDbContext _context;

public FinanceReportService(ApplicationDbContext context)
    {
        _context = context;
    }
private static FeeStatus GetEffectiveFeeStatus(
    FeeInvoice invoice,
    DateTime now)
{
    if (invoice.BalanceAmount <= 0)
        return FeeStatus.Paid;

    if (invoice.DueDate < now)
        return FeeStatus.Overdue;

    if (invoice.PaidAmount > 0)
        return FeeStatus.PartiallyPaid;

    return FeeStatus.Pending;
}
    public async Task<FinanceReportDashboardResponse> GetDashboardAsync()
    {
        var invoices = await _context.FeeInvoices
            .Where(x => !x.IsDeleted)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .ToListAsync();

        var payments = await _context.FeePayments
            .Where(x => !x.IsDeleted)
            .Include(x => x.FeeInvoice)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Student)
            .Include(x => x.FeeInvoice)
                .ThenInclude(x => x.Enrollment)
                    .ThenInclude(x => x.Course)
            .Include(x => x.PaymentMethod)
            .ToListAsync();

        // ---------------------------------------------------------
        // Summary
        // ---------------------------------------------------------

        var totalRevenue = invoices.Sum(x => x.Amount);
        var collectedAmount = payments.Sum(x => x.Amount);

        var pendingAmount = invoices
            .Where(x => x.BalanceAmount > 0)
            .Sum(x => x.BalanceAmount);

            var customerCredit = invoices
    .Where(x => x.BalanceAmount < 0)
    .Sum(x => -x.BalanceAmount);

        var monthlyAverageCollection = payments.Count == 0
            ? 0
            : payments
                .GroupBy(x => new
                {
                    x.PaymentDate.Year,
                    x.PaymentDate.Month
                })
                .Average(g => g.Sum(x => x.Amount));

        // ---------------------------------------------------------
        // Revenue Trend
        // Actual payments collected, grouped by month
        // ---------------------------------------------------------

        var revenueTrend = payments
            .GroupBy(x => new
            {
                x.PaymentDate.Year,
                x.PaymentDate.Month
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new RevenueTrendDto
            {
                Month = new DateTime(
                    g.Key.Year,
                    g.Key.Month,
                    1).ToString("MMM yyyy"),

                Amount = g.Sum(x => x.Amount)
            })
            .ToList();

        // ---------------------------------------------------------
        // Payment Status Distribution
        // Based on invoice status
        // ---------------------------------------------------------

        var now = DateTime.UtcNow;

var paymentStatusDistribution = Enum.GetValues<FeeStatus>()
    .Select(status => new PaymentStatusDistributionDto
    {
        Status = status.ToString(),
        Count = invoices.Count(x =>
            GetEffectiveFeeStatus(x, now) == status)
    })
    .ToList();

        // ---------------------------------------------------------
        // Course Revenue
        // Actual payments grouped by course
        // ---------------------------------------------------------

        var courseRevenue = payments
            .Where(x =>
                x.FeeInvoice.Enrollment != null &&
                x.FeeInvoice.Enrollment.Course != null)
            .GroupBy(x => new
            {
                x.FeeInvoice.Enrollment.CourseId,
                x.FeeInvoice.Enrollment.Course.CourseName
            })
            .OrderByDescending(g => g.Sum(x => x.Amount))
            .Select(g => new CourseRevenueDto
            {
                CourseName = g.Key.CourseName,
                Revenue = g.Sum(x => x.Amount)
            })
            .ToList();

        // ---------------------------------------------------------
        // Outstanding Fees
        // ---------------------------------------------------------

        var outstandingFees = invoices
            .Where(x => x.BalanceAmount > 0)
            .OrderBy(x => x.DueDate)
            .Select(x => new OutstandingFeeDto
            {
                StudentName =
                    x.Enrollment.Student.FullName,

                CourseName =
                    x.Enrollment.Course.CourseName,

                AmountDue = x.BalanceAmount,

                DueDate =
                    DateOnly.FromDateTime(x.DueDate),

                Status =
    GetEffectiveFeeStatus(x, now).ToString()
            })
            .ToList();

        // ---------------------------------------------------------
        // Recent Payments
        // ---------------------------------------------------------

        var recentPayments = payments
            .OrderByDescending(x => x.PaymentDate)
            .Take(10)
            .Select(x => new RecentPaymentDto
            {
                StudentName =
                    x.FeeInvoice.Enrollment.Student.FullName,

                Amount = x.Amount,

                PaymentDate =
                    DateOnly.FromDateTime(x.PaymentDate),

                PaymentMethod =
                    x.PaymentMethod.Name
            })
            .ToList();

        return new FinanceReportDashboardResponse
        {
            TotalRevenue = totalRevenue,
            CollectedAmount = collectedAmount,
            PendingAmount = pendingAmount,
            MonthlyAverageCollection = monthlyAverageCollection,

            RevenueTrend = revenueTrend,

            PaymentStatusDistribution =
                paymentStatusDistribution,

            CourseRevenue =
                courseRevenue,

            OutstandingFees =
                outstandingFees,

            RecentPayments =
                recentPayments,

                CustomerCredit = customerCredit
        };
    }
}