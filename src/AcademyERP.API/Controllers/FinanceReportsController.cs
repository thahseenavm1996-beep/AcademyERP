using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/reports/finance")]
[Authorize]
public class FinanceReportsController : ControllerBase
{
    private readonly IFinanceReportService _service;


    public FinanceReportsController(
        IFinanceReportService service)
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