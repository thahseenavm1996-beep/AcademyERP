namespace AcademyERP.Application.DTOs.FinanceReports;

public class FinanceReportDashboardResponse
{
    public decimal TotalRevenue { get; set; }

    public decimal CollectedAmount { get; set; }

    public decimal PendingAmount { get; set; }

    public decimal MonthlyAverageCollection { get; set; }


    public List<RevenueTrendDto> RevenueTrend { get; set; }
        = new();


    public List<PaymentStatusDistributionDto> PaymentStatusDistribution { get; set; }
        = new();


    public List<CourseRevenueDto> CourseRevenue { get; set; }
        = new();


    public List<OutstandingFeeDto> OutstandingFees { get; set; }
        = new();


    public List<RecentPaymentDto> RecentPayments { get; set; }
        = new();
}