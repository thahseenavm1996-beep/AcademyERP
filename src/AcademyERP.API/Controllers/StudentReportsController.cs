using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcademyERP.Infrastructure.Services;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/reports/students")]
[Authorize]
public class StudentReportsController : ControllerBase
{
    private readonly IStudentReportService _service;
private readonly StudentReportDashboardService _dashboardService;

    public StudentReportsController(
    IStudentReportService service,
    StudentReportDashboardService dashboardService)
{
    _service = service;
    _dashboardService = dashboardService;
}



    [HttpGet("{studentId:guid}")]
    public async Task<IActionResult> Get(
        Guid studentId)
    {
        var result =
            await _service.GetStudentReportAsync(studentId);


        if (result == null)
            return NotFound();


        return Ok(result);
    }
    [HttpGet("dashboard")]
public async Task<IActionResult> GetDashboard()
{
    var result =
        await _dashboardService.GetDashboardAsync();

    return Ok(result);
}
}