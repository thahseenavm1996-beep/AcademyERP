using AcademyERP.Application.Constants;
using AcademyERP.Application.Services;
using AcademyERP.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AcademyERP.Application.Interfaces;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/teacher/dashboard")]
[Authorize(Roles = Roles.Teacher + "," + Roles.Administrator + "," + Roles.SuperAdmin)]
public class TeacherDashboardController : ControllerBase
{
    private readonly ITeacherDashboardService _dashboardService;
    private readonly ICurrentUserService _currentUserService;

    public TeacherDashboardController(
        ITeacherDashboardService dashboardService,
        ICurrentUserService currentUserService)
    {
        _dashboardService = dashboardService;
        _currentUserService = currentUserService;
    }


    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var applicationUserId = _currentUserService.UserId;

        var teacherId = _currentUserService.UserId;

if (!teacherId.HasValue)
{
    return Unauthorized();
}

var dashboard = await _dashboardService
    .GetDashboardAsync(teacherId.Value);

return Ok(dashboard);

    }
}