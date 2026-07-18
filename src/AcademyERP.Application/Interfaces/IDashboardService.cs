using AcademyERP.Application.DTOs.Dashboard;

namespace AcademyERP.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryResponse> GetSummaryAsync();
}