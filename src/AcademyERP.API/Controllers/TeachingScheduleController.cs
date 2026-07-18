using AcademyERP.Application.DTOs.TeachingSchedules;
using AcademyERP.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyERP.API.Controllers;

[ApiController]
[Route("api/teachingschedules")]
[Authorize]
public class TeachingScheduleController : ControllerBase
{
    private readonly ITeachingScheduleService _service;

    public TeachingScheduleController(
        ITeachingScheduleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _service.GetAllAsync();

        return Ok(schedules);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTeachingScheduleRequest request)
    {
        var schedule = await _service.CreateAsync(request);

        return Ok(schedule);
    }
}