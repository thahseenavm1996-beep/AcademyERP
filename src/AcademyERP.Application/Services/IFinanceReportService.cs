using AcademyERP.Application.DTOs.FinanceReports;

namespace AcademyERP.Application.Services;

public interface IFinanceReportService
{
    Task<FinanceReportDashboardResponse> GetDashboardAsync();
}