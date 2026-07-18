using AcademyERP.Application.Constants;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/teacher/dashboard")]
[Authorize(Roles = Roles.Teacher)]
public class TeacherDashboardController : ControllerBase
{
    private readonly ITeacherDashboardService _dashboardService;

    public TeacherDashboardController(
        ITeacherDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("{teacherId:guid}")]
    public async Task<IActionResult> Get(Guid teacherId)
    {
        var dashboard = await _dashboardService.GetDashboardAsync(teacherId);

        return Ok(dashboard);
    }
}