using AcademyERP.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/reports/teachers")]
public class TeacherReportsController : ControllerBase
{
    private readonly TeacherReportService _service;

    public TeacherReportsController(
        TeacherReportService service)
    {
        _service = service;
    }


    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _service.GetDashboardAsync();

        return Ok(result);
    }
}