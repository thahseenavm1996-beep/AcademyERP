using AcademyERP.Application.Services;
using AcademyERP.Application.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/reports/export")]
[Authorize]
public class ReportExportsController : ControllerBase
{
    private readonly IAttendanceReportService _attendanceReportService;
    private readonly IPdfService _pdfService;

    public ReportExportsController(
        IAttendanceReportService attendanceReportService,
        IPdfService pdfService)
    {
        _attendanceReportService = attendanceReportService;
        _pdfService = pdfService;
    }

    [HttpGet("attendance/pdf")]
    public async Task<IActionResult> DownloadAttendanceReport()
    {
        var report =
            await _attendanceReportService.GetDashboardAsync();

        var pdf =
            _pdfService.GenerateAttendanceReport(report);

        return File(
            pdf,
            "application/pdf",
            $"attendance-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.pdf");
    }
}