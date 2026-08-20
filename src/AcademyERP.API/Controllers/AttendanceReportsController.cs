using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/reports/attendance")]
[Authorize]
public class AttendanceReportsController : ControllerBase
{
    private readonly IAttendanceReportService _service;


    public AttendanceReportsController(
        IAttendanceReportService service)
    {
        _service = service;
    }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result =
            await _service.GetDashboardAsync();

        return Ok(result);
    }
}