using AcademyERP.Application.DTOs.FinanceReports;
using AcademyERP.Application.Services;

namespace AcademyERP.Infrastructure.Services;

public class FinanceReportService : IFinanceReportService
{
    public async Task<FinanceReportDashboardResponse> GetDashboardAsync()
    {
        var response = new FinanceReportDashboardResponse();


        // Temporary dashboard data
        // Will be replaced with EF Core queries later


        response.TotalRevenue = 450000;

        response.CollectedAmount = 415000;

        response.PendingAmount = 35000;

        response.MonthlyAverageCollection = 90000;



        response.RevenueTrend =
        [
            new()
            {
                Month = "Jan",
                Amount = 70000
            },

            new()
            {
                Month = "Feb",
                Amount = 85000
            },

            new()
            {
                Month = "Mar",
                Amount = 90000
            },

            new()
            {
                Month = "Apr",
                Amount = 110000
            },

            new()
            {
                Month = "May",
                Amount = 120000
            }
        ];



        response.PaymentStatusDistribution =
        [
            new()
            {
                Status = "Paid",
                Count = 230
            },

            new()
            {
                Status = "Pending",
                Count = 20
            },

            new()
            {
                Status = "Overdue",
                Count = 10
            }
        ];



        response.CourseRevenue =
        [
            new()
            {
                CourseName = "Quran",
                Revenue = 200000
            },

            new()
            {
                CourseName = "Hifz",
                Revenue = 120000
            },

            new()
            {
                CourseName = "Arabic",
                Revenue = 80000
            },

            new()
            {
                CourseName = "Fiqh",
                Revenue = 50000
            }
        ];



        response.OutstandingFees =
        [
            new()
            {
                StudentName = "Ahmed",
                CourseName = "Quran",
                AmountDue = 2000,
                DueDate = new DateOnly(2026,8,15),
                Status = "Pending"
            },

            new()
            {
                StudentName = "Fatima",
                CourseName = "Hifz",
                AmountDue = 3000,
                DueDate = new DateOnly(2026,8,20),
                Status = "Overdue"
            }
        ];



        response.RecentPayments =
        [
            new()
            {
                StudentName = "Muhammad Hasan",
                Amount = 3000,
                PaymentDate = new DateOnly(2026,8,10),
                PaymentMethod = "UPI"
            },

            new()
            {
                StudentName = "Aisha",
                Amount = 2500,
                PaymentDate = new DateOnly(2026,8,5),
                PaymentMethod = "Card"
            }
        ];


        return response;
    }
}